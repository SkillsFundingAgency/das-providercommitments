using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadReviewRequestToReviewViewModelMapper : IMapper<FileUploadReviewRequest, FileUploadReviewViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<FileUploadReviewRequestToReviewViewModelMapper> _logger;
        private readonly IPolicyAuthorizationWrapper _policyAuthorizationWrapper;        
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUploadReviewRequestToReviewViewModelMapper(ILogger<FileUploadReviewRequestToReviewViewModelMapper> logger, ICommitmentsApiClient commitmentApiClient, 
            ICacheService cacheService, IEncodingService encodingService, IPolicyAuthorizationWrapper policyAuthorizationWrapper, IHttpContextAccessor httpContextAccessor)
        {
            _commitmentsApiClient = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
            _logger = logger;
            _policyAuthorizationWrapper = policyAuthorizationWrapper;            
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FileUploadReviewViewModel> Map(FileUploadReviewRequest source)
        {
            var result = new FileUploadReviewViewModel();
            result.ProviderId = source.ProviderId;
            result.CacheRequestId = source.CacheRequestId;
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);

            result.CanApprove = await _policyAuthorizationWrapper.IsAuthorized(_httpContextAccessor.HttpContext.User, PolicyNames.HasContributorWithApprovalOrAbovePermission);

            var groupedByEmployers = csvRecords.GroupBy(x => x.AgreementId);

            
            foreach (var employer in groupedByEmployers)
            {
                var employerDetail = new FileUploadReviewEmployerDetails();
                var publicAccountLegalEntityId =  _encodingService.Decode(employer.Key, EncodingType.PublicAccountLegalEntityId);
                employerDetail.EmployerName =  (await _commitmentsApiClient.GetAccountLegalEntity(publicAccountLegalEntityId)).AccountName;
                employerDetail.AgreementId = employer.Key;

                employerDetail.CohortDetails = new List<FileUploadReviewCohortDetail>();

                var cohortGroups = employer.GroupBy(x => x.CohortRef);

                var cohortDetailfromCsvList =  new List<FileUploadReviewCohortDetail>();
                var cohortDetailfromCsv = new FileUploadReviewCohortDetail();
                //get the records from CSV file
                foreach (var cohortGroup in cohortGroups)
                {
                    cohortDetailfromCsv.CohortRef = cohortGroup.Key;
                    cohortDetailfromCsv.NumberOfApprentices = cohortGroup.Count();
                    cohortDetailfromCsv.TotalCost = cohortGroup.Sum(x => int.Parse(x.TotalPrice));
                    cohortDetailfromCsvList.Add(cohortDetailfromCsv);
                }

                var cohortDetailfromDbList = new List<FileUploadReviewCohortDetail>();
                var cohortDetailfromDb = new FileUploadReviewCohortDetail();
                //get the records from database
                foreach (var cohortGroup in cohortGroups)
                {
                    //Get CohortId by CohortReference
                    var cohortId = _encodingService.Decode(cohortGroup.Key, EncodingType.CohortReference);
                    var response =await _commitmentsApiClient.GetDraftApprenticeships(cohortId);

                    cohortDetailfromDb.CohortRef = cohortGroup.Key;
                    cohortDetailfromDb.NumberOfApprentices = response.DraftApprenticeships.Count();
                    cohortDetailfromDb.TotalCost = response.DraftApprenticeships.Sum(x => x.Cost ?? 0);
                    cohortDetailfromDbList.Add(cohortDetailfromDb);
                }

                var newList = cohortDetailfromCsvList.Concat(cohortDetailfromDbList).ToList();

                

                /*cohortDetailfromCsvList.AddRange(
                        cohortDetailfromDbList.Select(
                        m =>
                           new FileUploadReviewCohortDetail
                           {
                               CohortRef = m.CohortRef,
                               NumberOfApprentices = m.NumberOfApprentices,
                               TotalCost = m.TotalCost
                           })
                        .OrderBy(x => x.CohortRef)
                        .ToArray());

                var result456 = cohortDetailfromCsvList;


                var newPriceHistory =
                    cohortDetailfromCsvList.Concat(cohortDetailfromDbList)
                   .Select(
                       m =>
                       new FileUploadReviewCohortDetail
                       {
                           CohortRef = m.CohortRef,
                           NumberOfApprentices = m.NumberOfApprentices,
                           TotalCost = m.TotalCost                           
                       })                   
                   .GroupBy(x => x.CohortRef)
                   .ToArray();

                var test = newPriceHistory;*/


                var cohortGroupsafterconcat = newList.GroupBy(x => x.CohortRef).Select(
                       m =>
                       new FileUploadReviewCohortDetail
                       {
                           CohortRef = m.Key,
                           NumberOfApprentices = m.Sum(x => x.NumberOfApprentices),
                           TotalCost = m.Sum(x => x.TotalCost)
                    }).ToList();


                foreach (var item in cohortGroupsafterconcat)
                {                   
                    employerDetail.CohortDetails.Add(item);
                }                

                result.EmployerDetails.Add(employerDetail);
            }



            //employerDetail.CohortDetails = new List<FileUploadReviewCohortDetail>();

            //var cohortGroups = employer.GroupBy(x => x.CohortRef);
            //foreach (var cohortGroup in cohortGroups)
            //{
            //    var cohortDetail = new FileUploadReviewCohortDetail();
            //    cohortDetail.CohortRef = cohortGroup.Key;
            //    cohortDetail.NumberOfApprentices = cohortGroup.Count();
            //    cohortDetail.TotalCost = cohortGroup.Sum(x => int.Parse(x.TotalPrice));
            //    employerDetail.CohortDetails.Add(cohortDetail);
            //}

            //result.EmployerDetails.Add(employerDetail);


            return result;
        }
    }
}


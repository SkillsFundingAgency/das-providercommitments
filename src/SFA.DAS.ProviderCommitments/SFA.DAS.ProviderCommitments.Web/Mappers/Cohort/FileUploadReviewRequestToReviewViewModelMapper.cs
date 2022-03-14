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

                var cohortDetails = new List<FileUploadReviewCohortDetail>();
               
                foreach (var cohortGroup in cohortGroups)
                {
                    //get the records from CSV file
                    var fileUploadCohortDetails = new FileUploadReviewCohortDetail
                    {
                        CohortRef = cohortGroup.Key,
                        NumberOfApprentices = cohortGroup.Count(),
                        TotalCost = cohortGroup.Sum(x => int.Parse(x.TotalPrice))
                    };
                    cohortDetails.Add(fileUploadCohortDetails);

                    //Get CohortId by CohortReference
                    var cohortId = _encodingService.Decode(cohortGroup.Key, EncodingType.CohortReference);
                    //Get the response from DB
                    var response = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);

                    if (response != null)
                    {
                        var existingCohortDetails = new FileUploadReviewCohortDetail
                        {
                            CohortRef = cohortGroup.Key,
                            NumberOfApprentices = response.DraftApprenticeships.Count,
                            TotalCost = response.DraftApprenticeships.Sum(x => x.Cost ?? 0)
                        };
                        cohortDetails.Add(existingCohortDetails);
                    }
                }

                employerDetail.CohortDetails.AddRange(cohortDetails.GroupBy(x => x.CohortRef).Select(
                       m =>
                       new FileUploadReviewCohortDetail
                       {
                           CohortRef = m.Key,
                           NumberOfApprentices = m.Sum(x => x.NumberOfApprentices),
                           TotalCost = m.Sum(x => x.TotalCost)
                       }).ToList());
                
                result.EmployerDetails.Add(employerDetail);
            }

            return result;
        }
    }
}
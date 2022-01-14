using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
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

        public FileUploadReviewRequestToReviewViewModelMapper(ICommitmentsApiClient commitmentApiClient,  ICacheService cacheService, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
        }

        public async Task<FileUploadReviewViewModel> Map(FileUploadReviewRequest source)
        {
            var result = new FileUploadReviewViewModel();
            result.ProviderId = source.ProviderId;
            result.CacheRequestId = source.CacheRequestId;
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());

           var groupedByEmployers = csvRecords.GroupBy(x => x.AgreementId);

            foreach (var employer in groupedByEmployers)
            {
                var employerDetail = new FileUploadReviewEmployerDetails();
                employerDetail.EmployerName =  (await _commitmentsApiClient.GetAccountLegalEntity(_encodingService.Decode(employer.Key, EncodingType.PublicAccountLegalEntityId))).AccountName;
                employerDetail.AgreementId = employer.Key;

                employerDetail.CohortDetails = new List<FileUploadReviewCohortDetail>();

                var cohortGroups = employer.GroupBy(x => x.CohortRef);
                foreach (var cohortGroup in cohortGroups)
                {
                    var cohortDetail = new FileUploadReviewCohortDetail();
                    cohortDetail.CohortRef = cohortGroup.Key;
                    cohortDetail.NumberOfApprentices = cohortGroup.Count();
                    cohortDetail.TotalCost = cohortGroup.Sum(x => int.Parse(x.TotalPrice));
                    employerDetail.CohortDetails.Add(cohortDetail);
                }

                result.EmployerDetails.Add(employerDetail);
            }

            return result;
        }
    }
}

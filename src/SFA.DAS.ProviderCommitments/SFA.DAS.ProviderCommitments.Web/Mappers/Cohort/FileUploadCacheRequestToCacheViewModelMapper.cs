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
    public class FileUploadCacheRequestToCacheViewModelMapper : IMapper<FileUploadCacheRequest, FileUploadCacheViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;

        public FileUploadCacheRequestToCacheViewModelMapper(ICommitmentsApiClient commitmentApiClient,  ICacheService cacheService, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
        }

        public async Task<FileUploadCacheViewModel> Map(FileUploadCacheRequest source)
        {
            var result = new FileUploadCacheViewModel();
            result.ProviderId = source.ProviderId;
            result.CacheRequestId = source.CacheRequestId;
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());

           var groupedByEmployers = csvRecords.GroupBy(x => x.AgreementId);

            foreach (var employer in groupedByEmployers)
            {
                var employerDetail = new FileUploadCacheEmployerDetails();
                employerDetail.EmployerName =  (await _commitmentsApiClient.GetAccountLegalEntity(_encodingService.Decode(employer.Key, EncodingType.PublicAccountLegalEntityId))).AccountName;
                employerDetail.AgreementId = employer.Key;

                employerDetail.CohortDetails = new List<FileUploadCacheCohortDetail>();

                var cohortGroups = employer.GroupBy(x => x.CohortRef);
                foreach (var cohortGroup in cohortGroups)
                {
                    var cohortDetail = new FileUploadCacheCohortDetail();
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

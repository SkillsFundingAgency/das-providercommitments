using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper : IMapper<FileUploadReviewViewModel, BulkUploadAddDraftApprenticeshipsRequest>
    {
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;
        public FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper(ICacheService cacheService, IEncodingService encodingService)
        {
            _cacheService = cacheService;
            _encodingService = encodingService;
        }

        public async Task<BulkUploadAddDraftApprenticeshipsRequest> Map(FileUploadReviewViewModel source)
        {
            var csVRecords = await _cacheService.GetFromCache<List<Models.Cohort.CsvRecord>>(source.CacheRequestId.ToString());
            await _cacheService.ClearCache(source.CacheRequestId.ToString());
            return new BulkUploadAddDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                BulkUploadDraftApprenticeships = csVRecords.Select(x => MapTo(x))
            };
        }

        private BulkUploadAddDraftApprenticeshipRequest MapTo(Models.Cohort.CsvRecord record)
        {
            return new BulkUploadAddDraftApprenticeshipRequest
            {
                Uln = record.ULN,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirthAsString = record.DateOfBirth,
                CostAsString = record.TotalPrice,
                ProviderRef = record.ProviderRef,
                StartDateAsString =record.StartDate,
                EndDateAsString = record.EndDate,
                CourseCode = record.StdCode,
                LegalEntityId = _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId),
                CohortId = _encodingService.Decode(record.CohortRef, EncodingType.CohortReference),
                Email = record.EmailAddress
            };
        }
    }
}

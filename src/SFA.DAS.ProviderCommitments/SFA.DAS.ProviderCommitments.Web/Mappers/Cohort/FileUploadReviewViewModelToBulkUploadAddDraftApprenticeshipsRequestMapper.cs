using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var csVRecrods = await _cacheService.GetFromCache<List<Models.Cohort.CsvRecord>>(source.CacheRequestId.ToString());
            await _cacheService.ClearCache(source.CacheRequestId.ToString());
            return new BulkUploadAddDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                BulkUploadDraftApprenticeships = csVRecrods.Select(x => MapTo(x))
            };
        }

        private BulkUploadAddDraftApprenticeshipRequest MapTo(Models.Cohort.CsvRecord record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");
            var learnerStartDate = GetValidDate(record.StartDate, "yyyy-MM-dd");
            var learnerEndDate = GetValidDate(record.EndDate, "yyyy-MM");

            return new BulkUploadAddDraftApprenticeshipRequest
            {
                Uln = record.ULN,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirth = dateOfBirth,
                Cost = int.Parse(record.TotalPrice),
                ProviderRef = record.ProviderRef,
                StartDate = new DateTime(learnerStartDate.Value.Year, learnerStartDate.Value.Month, 1),
                EndDate = learnerEndDate,
                CourseCode = record.StdCode,
                LegalEntityId = _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId),
                CohortId = _encodingService.Decode(record.CohortRef, EncodingType.CohortReference),
                Email = record.EmailAddress
            };
        }

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }
}

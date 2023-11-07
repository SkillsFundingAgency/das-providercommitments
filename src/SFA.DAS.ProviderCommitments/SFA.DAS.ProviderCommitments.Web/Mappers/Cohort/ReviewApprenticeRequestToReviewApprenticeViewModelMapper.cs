using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Globalization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ReviewApprenticeRequestToReviewApprenticeViewModelMapper : IMapper<FileUploadReviewApprenticeRequest, FileUploadReviewApprenticeViewModel>
    {
        private readonly IOuterApiService _outerApiService;
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;        
        private readonly ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper> _logger;

        public ReviewApprenticeRequestToReviewApprenticeViewModelMapper(ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper> logger,
            IOuterApiService commitmentApiClient, ICacheService cacheService, IEncodingService encodingService)
        {
            _outerApiService = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
            _logger = logger;            
        }

        public async Task<FileUploadReviewApprenticeViewModel> Map(FileUploadReviewApprenticeRequest source)
        {
            //Get CsvRecord details
            var cacheModel = await _cacheService.GetFromCache<FileUploadCacheModel>(source.CacheRequestId.ToString());
            var csvRecords = cacheModel.CsvRecords;
            var csvRecordsGroupedByCohort = csvRecords.Where(x => (!string.IsNullOrWhiteSpace(x.CohortRef) && x.CohortRef == source.CohortRef) ||
                                   (string.IsNullOrWhiteSpace(source.CohortRef) && string.IsNullOrWhiteSpace(x.CohortRef) && source.AgreementId == x.AgreementId));
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);

            //Get commitments from DB
            var existingCohortDetails = new List<ReviewApprenticeDetailsForExistingCohort>();
            var messageFromEmployer = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.CohortRef))
            {
                var cohortId = _encodingService.Decode(source.CohortRef, EncodingType.CohortReference);
                messageFromEmployer = (await _outerApiService.GetCohort(cohortId)).LatestMessageCreatedByEmployer;
                var existingDraftApprenticeshipsResponse = await _outerApiService.GetDraftApprenticeships(cohortId);
                if (existingDraftApprenticeshipsResponse != null)
                {
                    _logger.LogInformation("Total number of records from db: " + existingDraftApprenticeshipsResponse.DraftApprenticeships.Count);
                    existingCohortDetails = await MapExistingCohortDetails(existingDraftApprenticeshipsResponse);
                }
            }

            var publicAccountLegalEntityIdNew = _encodingService.Decode(csvRecordsGroupedByCohort.FirstOrDefault().AgreementId, EncodingType.PublicAccountLegalEntityId);
            var result = new FileUploadReviewApprenticeViewModel
            {
                ProviderId = source.ProviderId,
                CohortRef = (!string.IsNullOrWhiteSpace(source.CohortRef)) ? source.CohortRef : string.Empty,
                CacheRequestId = source.CacheRequestId,
                ExistingCohortDetails = existingCohortDetails,
                FileUploadCohortDetails = await MapFileUploadCohortDetails(csvRecordsGroupedByCohort),
                LegalEntityName = (await _outerApiService.GetAccountLegalEntity(publicAccountLegalEntityIdNew)).LegalEntityName,
                MessageFromEmployer = messageFromEmployer
            };

            return result;
        }       

        private async Task<List<ReviewApprenticeDetailsForFileUploadCohort>> MapFileUploadCohortDetails(IEnumerable<CsvRecord> csvRecordsGroupedByCohort)
        {
            var reviewApprenticeDetailsForFileUploadCohort = new List<ReviewApprenticeDetailsForFileUploadCohort>();

            foreach (var csvRecord in csvRecordsGroupedByCohort)
            {
                var courseDetails = await _outerApiService.GetStandardDetails(csvRecord.StdCode);
                var dateOfBirth = GetValidDate(csvRecord.DateOfBirth, "yyyy-MM-dd");
                var apprenticeStartDate = GetValidDate(csvRecord.StartDate, "yyyy-MM-dd");
                var apprenticeEndDate = GetValidDate(csvRecord.EndDate, "yyyy-MM");

                var fileUploadApprenticeDetail = new ReviewApprenticeDetailsForFileUploadCohort
                {
                    Name = $"{csvRecord.GivenNames} {csvRecord.FamilyName}",
                    TrainingCourse = courseDetails.Title,
                    ULN = csvRecord.ULN,
                    DateOfBirth = dateOfBirth.Value.ToString("d MMM yyyy"),
                    Email = csvRecord.EmailAddress,
                    TrainingDates = $"{apprenticeStartDate.Value:MMM yyyy} to {apprenticeEndDate.Value:MMM yyyy} ",
                    Price = int.Parse(csvRecord.TotalPrice),
                    FundingBandCap = GetFundingBandCap(courseDetails, apprenticeStartDate.Value.Date)
                };

                reviewApprenticeDetailsForFileUploadCohort.Add(fileUploadApprenticeDetail);
            }

            return reviewApprenticeDetailsForFileUploadCohort;
        }

        private async Task<List<ReviewApprenticeDetailsForExistingCohort>> MapExistingCohortDetails(GetDraftApprenticeshipsResult response)
        {
            var reviewApprenticeDetailsForExistingCohort = new List<ReviewApprenticeDetailsForExistingCohort>();

            foreach (var item in response.DraftApprenticeships)
            {   
                var course = await _outerApiService.GetStandardDetails(item.CourseCode);

                var apprenticeDetail = new ReviewApprenticeDetailsForExistingCohort
                {
                    Name = $"{item.FirstName} {item.LastName}",
                    TrainingCourse = item.CourseName,
                    ULN = item.Uln,
                    DateOfBirth = item.DateOfBirth.Value.ToString("d MMM yyyy"),
                    Email = item.Email,
                    TrainingDates = $"{item.StartDate.Value:MMM yyyy} to {item.EndDate.Value:MMM yyyy} ",
                    Price = item.Cost ?? 0,
                    FundingBandCapForExistingCohort = GetFundingBandCap(course, item.StartDate.Value.Date)
                };

                reviewApprenticeDetailsForExistingCohort.Add(apprenticeDetail);
            }

            return reviewApprenticeDetailsForExistingCohort;
        }

        private static DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }

        private static int? GetFundingBandCap(GetStandardResponse course, DateTime? startDate)
        {
            if (startDate == null)
            {
                return null;
            }

            if (course == null)
            {
                return null;
            }

            var cap = course.FundingCapOn(startDate.Value);

            if (cap > 0)
            {
                return cap;
            }

            return null;
        }
    }
}
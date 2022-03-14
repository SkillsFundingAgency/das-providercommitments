using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ReviewApprenticeRequestToReviewApprenticeViewModelMapper : IMapper<FileUploadReviewApprenticeRequest, FileUploadReviewApprenticeViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;        
        private readonly ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper> _logger;

        public ReviewApprenticeRequestToReviewApprenticeViewModelMapper(ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper> logger, 
            ICommitmentsApiClient commitmentApiClient, ICacheService cacheService, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
            _logger = logger;            
        }

        public async Task<FileUploadReviewApprenticeViewModel> Map(FileUploadReviewApprenticeRequest source)
        {
            //Get CsvRecord details
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());            
            var csvRecordsGroupedByCohort = csvRecords.Where(x => (!string.IsNullOrWhiteSpace(x.CohortRef) && x.CohortRef == source.CohortRef) ||
                                   (string.IsNullOrWhiteSpace(source.CohortRef) && string.IsNullOrWhiteSpace(x.CohortRef) && source.AgreementId == x.AgreementId));
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);

            //Get commitments from DB
            var existingCohortDetails = new List<ReviewApprenticeDetailsForExistingCohort>();
            var messageFromEmployer = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.CohortRef))
            {
                var cohortId = _encodingService.Decode(source.CohortRef, EncodingType.CohortReference);
                messageFromEmployer = (await _commitmentsApiClient.GetCohort(cohortId)).LatestMessageCreatedByEmployer;
                var existingDraftApprenticeshipsResponse = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);
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
                EmployerName = (await _commitmentsApiClient.GetAccountLegalEntity(publicAccountLegalEntityIdNew)).AccountName
            };

            return result;
        }       

        private async Task<List<ReviewApprenticeDetailsForFileUploadCohort>> MapFileUploadCohortDetails(IEnumerable<CsvRecord> csvRecordsGroupedByCohort)
        {
            var reviewApprenticeDetailsForFileUploadCohort = new List<ReviewApprenticeDetailsForFileUploadCohort>();

            foreach (var csvRecord in csvRecordsGroupedByCohort)
            {
                var courseDetails = await _commitmentsApiClient.GetTrainingProgramme(csvRecord.StdCode);
                var dateOfBirth = GetValidDate(csvRecord.DateOfBirth, "yyyy-MM-dd");
                var apprenticeStartDate = GetValidDate(csvRecord.StartDate, "yyyy-MM-dd");
                var apprenticeEndDate = GetValidDate(csvRecord.EndDate, "yyyy-MM");

                var fileUploadApprenticeDetail = new ReviewApprenticeDetailsForFileUploadCohort
                {
                    Name = $"{csvRecord.GivenNames} {csvRecord.FamilyName}",
                    TrainingCourse = courseDetails.TrainingProgramme.Name,
                    ULN = csvRecord.ULN,
                    DateOfBirth = dateOfBirth.Value.ToString("d MMM yyyy"),
                    Email = csvRecord.EmailAddress,
                    TrainingDates = $"{apprenticeStartDate.Value:MMM yyyy} to {apprenticeEndDate.Value:MMM yyyy} ",
                    Price = int.Parse(csvRecord.TotalPrice),
                    FundingBandCap = GetFundingBandCap(courseDetails.TrainingProgramme, apprenticeStartDate.Value.Date)
                };

                reviewApprenticeDetailsForFileUploadCohort.Add(fileUploadApprenticeDetail);
            }

            return reviewApprenticeDetailsForFileUploadCohort;
        }

        private async Task<List<ReviewApprenticeDetailsForExistingCohort>> MapExistingCohortDetails(GetDraftApprenticeshipsResponse response)
        {
            var reviewApprenticeDetailsForExistingCohort = new List<ReviewApprenticeDetailsForExistingCohort>();

            foreach (var item in response.DraftApprenticeships)
            {   
                var course = await _commitmentsApiClient.GetTrainingProgramme(item.CourseCode);

                var apprenticeDetail = new ReviewApprenticeDetailsForExistingCohort
                {
                    Name = $"{item.FirstName} {item.LastName}",
                    TrainingCourse = item.CourseName,
                    ULN = item.Uln,
                    DateOfBirth = item.DateOfBirth.Value.ToString("d MMM yyyy"),
                    Email = item.Email,
                    TrainingDates = $"{item.StartDate.Value:MMM yyyy} to {item.EndDate.Value:MMM yyyy} ",
                    Price = item.Cost ?? 0,
                    FundingBandCapForExistingCohort = GetFundingBandCap(course.TrainingProgramme, item.StartDate.Value.Date)
                };

                reviewApprenticeDetailsForExistingCohort.Add(apprenticeDetail);
            }

            return reviewApprenticeDetailsForExistingCohort;
        }

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }

        private int? GetFundingBandCap(TrainingProgramme course, DateTime? startDate)
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
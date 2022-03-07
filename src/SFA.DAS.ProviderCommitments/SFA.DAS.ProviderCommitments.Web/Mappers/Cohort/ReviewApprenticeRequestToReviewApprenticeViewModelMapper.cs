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

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ReviewApprenticeRequestToReviewApprenticeViewModelMapper : IMapper<ReviewApprenticeRequest, ReviewApprenticeViewModel>
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

        public async Task<ReviewApprenticeViewModel> Map(ReviewApprenticeRequest source)
        {
            var result = new ReviewApprenticeViewModel
            {
                ProviderId = source.ProviderId,
                CacheRequestId = source.CacheRequestId,
                ExistingCohortDetails = new List<ReviewApprenticeDetails>(),
                FileUploadCohortDetails = new List<FileUploadReviewApprenticeDetails>(),
            };

            //Get CsvRecord details
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            var csvRecordsGroupedByCohort = csvRecords.Where(x => x.CohortRef == source.CohortRef);
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);
            var fileUploadRecords = new List<FileUploadReviewApprenticeDetails>();           

            foreach (var csvRecord in csvRecordsGroupedByCohort)
            {
                var courseDetails = await _commitmentsApiClient.GetTrainingProgramme(csvRecord.StdCode);
                var dateOfBirth = GetValidDate(csvRecord.DateOfBirth, "yyyy-MM-dd");
                var apprenticeStartDate = GetValidDate(csvRecord.StartDate, "yyyy-MM-dd");
                var apprenticeEndDate = GetValidDate(csvRecord.EndDate, "yyyy-MM");

                var fileUploadApprenticeDetail = new FileUploadReviewApprenticeDetails
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

                result.FileUploadCohortDetails.Add(fileUploadApprenticeDetail);
                fileUploadRecords.Add(fileUploadApprenticeDetail);                
            }

            //Get CohortId by CohortReference -- commitments from DB
            var cohortId = _encodingService.Decode(source.CohortRef, EncodingType.CohortReference);
            var response = await _commitmentsApiClient.GetDraftApprenticeships(cohortId);
            var existingRecords = new List<ReviewApprenticeDetails>();
            _logger.LogInformation("Total number of records from db: " + response.DraftApprenticeships.Count);           

            foreach (var item in response.DraftApprenticeships)
            {
                //var courseDetails = await _commitmentsApiClient.GetTrainingProgramme(csvRecord.StdCode);
                var course = await _commitmentsApiClient.GetTrainingProgramme(item.CourseCode);

                var apprenticeDetail = new ReviewApprenticeDetails
                {
                    Name = $"{item.FirstName} {item.LastName}",
                    TrainingCourse = item.CourseName,
                    ULN = item.Uln,
                    DateOfBirth = item.DateOfBirth.ToString(),
                    Email = item.Email,
                    TrainingDates = $"{item.StartDate.Value:MMM yyyy} to {item.EndDate.Value:MMM yyyy} ",
                    Price = item.Cost ?? 0,
                    FundingBandCap = GetFundingBandCap(course.TrainingProgramme, item.StartDate.Value.Date)
                };
                result.ExistingCohortDetails.Add(apprenticeDetail);
                existingRecords.Add(apprenticeDetail);
            }

            var publicAccountLegalEntityIdNew = _encodingService.Decode(csvRecordsGroupedByCohort.FirstOrDefault().AgreementId, EncodingType.PublicAccountLegalEntityId);
            result.EmployerName = (await _commitmentsApiClient.GetAccountLegalEntity(publicAccountLegalEntityIdNew)).AccountName;
            result.CohortRef = source.CohortRef;
            result.TotalApprentices = fileUploadRecords.Count() + existingRecords.Count(); //newList.Count();
            result.TotalCost = fileUploadRecords.Sum(x => x.Price) + existingRecords.Sum(x => x.Price);   //newList.Sum(x => x.Price);
            result.CsvTotalApprenticesText = $"{fileUploadRecords.Count()} apprentice(s) to be added from CSV file";
            result.DbTotalApprenticesText = $"{existingRecords.Count()} apprentice(s) previously added to this cohort";
          
            if (!string.IsNullOrWhiteSpace(result.CohortRef))
            {
                var commitmentId = _encodingService.Decode(result.CohortRef, EncodingType.CohortReference);
                result.MessageFromEmployer = (await _commitmentsApiClient.GetCohort(commitmentId)).LatestMessageCreatedByEmployer;
            }

            return result;
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

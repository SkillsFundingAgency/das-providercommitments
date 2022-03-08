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

        public ReviewApprenticeRequestToReviewApprenticeViewModelMapper(ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper> logger, ICommitmentsApiClient commitmentApiClient, ICacheService cacheService, IEncodingService encodingService)
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
                CohortDetails = new List<ReviewApprenticeDetails>()
            };
            
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);        

            var filterByCohorts = csvRecords.Where(x => x.CohortRef == source.CohortRef || 
            (string.IsNullOrWhiteSpace(source.CohortRef) && string.IsNullOrWhiteSpace(x.CohortRef)));

            foreach (var record in filterByCohorts)
            {   
                var publicAccountLegalEntityId = _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId);
                var courseDetails = await _commitmentsApiClient.GetTrainingProgramme(record.StdCode);

                var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");
                var apprenticeStartDate = GetValidDate(record.StartDate, "yyyy-MM-dd");
                var apprenticeEndDate = GetValidDate(record.EndDate, "yyyy-MM");
                
                result.EmployerName = (await _commitmentsApiClient.GetAccountLegalEntity(publicAccountLegalEntityId)).AccountName;
                result.CohortRef = !string.IsNullOrWhiteSpace(record.CohortRef) ? record.CohortRef : "This will be created when you save or send to employers";
                result.TotalApprentices = filterByCohorts.Count();
                result.TotalCost = filterByCohorts.Sum(x => int.Parse(x.TotalPrice));                

                var apprenticeDetail = new ReviewApprenticeDetails
                {                  
                    Name = $"{record.GivenNames} {record.FamilyName}",
                    TrainingCourse = courseDetails.TrainingProgramme.Name, 
                    ULN = record.ULN,
                    DateOfBirth =  dateOfBirth.Value.ToString("d MMM yyyy"),
                    Email = record.EmailAddress,
                    TrainingDates =  $"{apprenticeStartDate.Value:MMM yyyy} to {apprenticeEndDate.Value:MMM yyyy} ",
                    Price = int.Parse(record.TotalPrice),
                    FundingBandCap = GetFundingBandCap(courseDetails.TrainingProgramme, apprenticeStartDate.Value.Date)                    
                };

                result.CohortDetails.Add(apprenticeDetail);
            }

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

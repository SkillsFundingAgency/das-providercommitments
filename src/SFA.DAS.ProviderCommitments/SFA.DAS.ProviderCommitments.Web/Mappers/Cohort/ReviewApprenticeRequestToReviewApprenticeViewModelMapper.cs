using Microsoft.Extensions.Logging;
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
    public class ReviewApprenticeRequestToReviewApprenticeViewModelMapper : IMapper<ReviewApprenticeRequest, ReviewApprenticeViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<FileUploadReviewRequestToReviewViewModelMapper> _logger;

        public ReviewApprenticeRequestToReviewApprenticeViewModelMapper(ILogger<FileUploadReviewRequestToReviewViewModelMapper> logger, ICommitmentsApiClient commitmentApiClient, ICacheService cacheService, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentApiClient;
            _cacheService = cacheService;
            _encodingService = encodingService;
            _logger = logger;
        }


        public async Task<ReviewApprenticeViewModel> Map(ReviewApprenticeRequest source)
        {
            var result = new ReviewApprenticeViewModel();
            result.ProviderId = source.ProviderId;
            result.CacheRequestId = source.CacheRequestId;
            var csvRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            _logger.LogInformation("Total number of records from cache: " + csvRecords.Count);

            var groupedByCohort = csvRecords.Where(x => x.CohortRef == "VLB8N4");

            foreach (var cohort in groupedByCohort)
            {
                // var employerDetail = new ReviewApprenticeDetails();
                //var publicAccountLegalEntityId = _encodingService.Decode(employer.Key, EncodingType.PublicAccountLegalEntityId);
                //result.EmployerName = (await _commitmentsApiClient.GetAccountLegalEntity(publicAccountLegalEntityId)).AccountName;

                result.CohortDetails.Add(new ReviewApprenticeDetails
                {
                    Email = cohort.EmailAddress,
                    DateOfBirth = cohort.DateOfBirth,
                    Name = $"{cohort.FamilyName} {cohort.GivenNames}",
                    Price = cohort.TotalPrice,
                    TrainingCourse = cohort.StdCode,
                    TrainingDates = $" {cohort.StartDate} to {cohort.EndDate}",
                    ULN = cohort.ULN                    
                 });               

            }

            return result;



        }
    }
}

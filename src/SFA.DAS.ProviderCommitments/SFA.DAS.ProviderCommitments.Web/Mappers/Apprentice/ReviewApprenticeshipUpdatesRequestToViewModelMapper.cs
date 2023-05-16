using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ReviewApprenticeshipUpdatesRequestToViewModelMapper : IMapper<ReviewApprenticeshipUpdatesRequest, ReviewApprenticeshipUpdatesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiClient _apiClient;

        public ReviewApprenticeshipUpdatesRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IOuterApiClient apiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _apiClient = apiClient;
        }

        public async Task<ReviewApprenticeshipUpdatesViewModel> Map(ReviewApprenticeshipUpdatesRequest source)
        {
            var updatesTask = _commitmentsApiClient.GetApprenticeshipUpdates(source.ApprenticeshipId,
                   new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest { Status = CommitmentsV2.Types.ApprenticeshipUpdateStatus.Pending });

            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            bool isValidCourseCode = true;

            await Task.WhenAll(updatesTask, apprenticeshipTask);

            var updates = updatesTask.Result;
            var apprenticeship = apprenticeshipTask.Result;

            if (updates.ApprenticeshipUpdates.Count == 1)
            {
                var update = updates.ApprenticeshipUpdates.First();

                if (!string.IsNullOrWhiteSpace(update.TrainingName))
                {
                    var apiRequest = new CheckReviewApprenticeshipCourseRequest(source.ProviderId, source.ApprenticeshipId);
                    var apiResponse = await _apiClient.Get<CheckReviewApprenticeshipCourseResponse>(apiRequest);
                    isValidCourseCode = apiResponse?.IsValidCourseCode ?? false;
                }

                if (!string.IsNullOrWhiteSpace(update.FirstName + update.LastName))
                {
                    update.FirstName = string.IsNullOrWhiteSpace(update.FirstName) ? apprenticeship.FirstName : update.FirstName;
                    update.LastName = string.IsNullOrWhiteSpace(update.LastName) ? apprenticeship.LastName : update.LastName;
                }

                var vm = new ReviewApprenticeshipUpdatesViewModel
                {
                    ProviderName = apprenticeship.ProviderName,
                    EmployerName = apprenticeship.EmployerName,
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    IsValidCourseCode = isValidCourseCode,
                    ApprenticeshipUpdates = new BaseEdit
                    {
                        FirstName = update.FirstName,
                        LastName = update.LastName,
                        Email = update.Email,
                        DateOfBirth = update.DateOfBirth,
                        Cost = update.Cost,
                        StartDate = update.StartDate,
                        EndDate = update.EndDate,
                        CourseCode = update.TrainingCode,
                        CourseName = update.TrainingName,
                        Version = update.Version,
                        Option = update.Option,
                        DeliveryModel = update.DeliveryModel,
                        EmploymentEndDate = update.EmploymentEndDate,
                        EmploymentPrice = update.EmploymentPrice
                    },
                    OriginalApprenticeship = new BaseEdit
                    {
                        FirstName = apprenticeship.FirstName,
                        LastName = apprenticeship.LastName,
                        Email = apprenticeship.Email,
                        DateOfBirth = apprenticeship.DateOfBirth,
                        ULN = apprenticeship.Uln,
                        StartDate = apprenticeship.StartDate,
                        EndDate = apprenticeship.EndDate,
                        CourseCode = apprenticeship.CourseCode,
                        CourseName = apprenticeship.CourseName,
                        Version = apprenticeship.Version,
                        Option = apprenticeship.Option,
                        DeliveryModel = apprenticeship.DeliveryModel,
                        EmploymentEndDate = apprenticeship.EmploymentEndDate,
                        EmploymentPrice = apprenticeship.EmploymentPrice
                    }
                };

                if (update.Cost.HasValue)
                {
                    var priceEpisodes = await _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);
                    vm.OriginalApprenticeship.Cost = priceEpisodes.PriceEpisodes.GetPrice();
                }

                return vm;
            }

            throw new Exception("Multiple pending updates found");
        }
    }
}

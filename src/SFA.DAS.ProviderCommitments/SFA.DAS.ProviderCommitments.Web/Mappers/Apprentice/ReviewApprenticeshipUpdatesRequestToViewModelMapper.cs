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
            var apiRequest = new GetReviewApprenticeshipUpdatesRequest(source.ProviderId, source.ApprenticeshipId);
            var apprenticeship = await _apiClient.Get<GetReviewApprenticeshipUpdatesResponse>(apiRequest);

            if (!string.IsNullOrWhiteSpace(apprenticeship.ApprenticeshipUpdates.FirstName + apprenticeship.ApprenticeshipUpdates.LastName))
            {
                apprenticeship.ApprenticeshipUpdates.FirstName = string.IsNullOrWhiteSpace(apprenticeship.ApprenticeshipUpdates.FirstName) ? apprenticeship.OriginalApprenticeship.FirstName : apprenticeship.ApprenticeshipUpdates.FirstName;
                apprenticeship.ApprenticeshipUpdates.LastName = string.IsNullOrWhiteSpace(apprenticeship.ApprenticeshipUpdates.LastName) ? apprenticeship.OriginalApprenticeship.LastName : apprenticeship.ApprenticeshipUpdates.LastName;
            }

            var vm = new ReviewApprenticeshipUpdatesViewModel
            {
                ProviderName = apprenticeship.ProviderName,
                EmployerName = apprenticeship.EmployerName,
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                IsValidCourseCode = apprenticeship.IsValidCourseCode,
                ApprenticeshipUpdates = new BaseEdit
                {
                    FirstName = apprenticeship.ApprenticeshipUpdates.FirstName,
                    LastName = apprenticeship.ApprenticeshipUpdates.LastName,
                    Email = apprenticeship.ApprenticeshipUpdates.Email,
                    DateOfBirth = apprenticeship.ApprenticeshipUpdates.DateOfBirth,
                    Cost = apprenticeship.ApprenticeshipUpdates.Cost,
                    StartDate = apprenticeship.ApprenticeshipUpdates.StartDate,
                    EndDate = apprenticeship.ApprenticeshipUpdates.EndDate,
                    CourseCode = apprenticeship.ApprenticeshipUpdates.CourseCode,
                    CourseName = apprenticeship.ApprenticeshipUpdates.CourseName,
                    Version = apprenticeship.ApprenticeshipUpdates.Version,
                    Option = apprenticeship.ApprenticeshipUpdates.Option,
                    DeliveryModel = apprenticeship.ApprenticeshipUpdates.DeliveryModel,
                    EmploymentEndDate = apprenticeship.ApprenticeshipUpdates.EmploymentEndDate,
                    EmploymentPrice = apprenticeship.ApprenticeshipUpdates.EmploymentPrice
                },
                OriginalApprenticeship = new BaseEdit
                {
                    FirstName = apprenticeship.OriginalApprenticeship.FirstName,
                    LastName = apprenticeship.OriginalApprenticeship.LastName,
                    Email = apprenticeship.OriginalApprenticeship.Email,
                    DateOfBirth = apprenticeship.OriginalApprenticeship.DateOfBirth,
                    ULN = apprenticeship.OriginalApprenticeship.Uln,
                    StartDate = apprenticeship.OriginalApprenticeship.StartDate,
                    EndDate = apprenticeship.OriginalApprenticeship.EndDate,
                    CourseCode = apprenticeship.OriginalApprenticeship.CourseCode,
                    CourseName = apprenticeship.OriginalApprenticeship.CourseName,
                    Version = apprenticeship.OriginalApprenticeship.Version,
                    Option = apprenticeship.OriginalApprenticeship.Option,
                    DeliveryModel = apprenticeship.OriginalApprenticeship.DeliveryModel,
                    EmploymentEndDate = apprenticeship.OriginalApprenticeship.EmploymentEndDate,
                    EmploymentPrice = apprenticeship.OriginalApprenticeship.EmploymentPrice,
                    Cost = apprenticeship.OriginalApprenticeship.Cost
                }
            };

            return vm;
        }
    }
}

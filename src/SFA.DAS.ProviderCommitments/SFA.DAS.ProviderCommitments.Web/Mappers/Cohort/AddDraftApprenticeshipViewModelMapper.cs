using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ITempDataStorageService _tempData;

        public AddDraftApprenticeshipViewModelMapper(IOuterApiClient outerApiClient, ITempDataStorageService tempDataStorageService)
        {
            _outerApiClient = outerApiClient;
            _tempData = tempDataStorageService;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var result = _tempData.RetrieveFromCache<AddDraftApprenticeshipViewModel>();
            _tempData.RemoveFromCache<AddDraftApprenticeshipViewModel>();

            if (result == null)
            {
                var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, source.AccountLegalEntityId, source.CourseCode);
                var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

                result = new AddDraftApprenticeshipViewModel
                {
                    ProviderId = source.ProviderId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    AccountLegalEntityId = source.AccountLegalEntityId,
                    StartDate = new MonthYearModel(source.StartMonthYear),
                    ReservationId = source.ReservationId.Value,
                    CourseCode = source.CourseCode,
                    DeliveryModel = source.DeliveryModel,
                    Courses = null,
                    Employer = apiResponse.LegalEntityName,
                    HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                    IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
                };
            }
            else
            {
                result.CourseCode = source.CourseCode;
                result.DeliveryModel = source.DeliveryModel;
                result.IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot;
            }

            return result;
        }
    }
}
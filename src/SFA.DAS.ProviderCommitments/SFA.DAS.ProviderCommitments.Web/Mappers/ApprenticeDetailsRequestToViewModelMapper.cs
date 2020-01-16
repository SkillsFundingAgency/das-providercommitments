using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ApprenticeDetailsRequestToViewModelMapper : IMapper<ApprenticeDetailsRequest, ApprenticeDetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;

        public ApprenticeDetailsRequestToViewModelMapper(ICommitmentsApiClient commitmentApiClient)
        {
            _commitmentApiClient = commitmentApiClient;
        }

        public Task<ApprenticeDetailsViewModel> Map(ApprenticeDetailsRequest source)
        {
            return Task.FromResult(new ApprenticeDetailsViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                Name = "Name User"
            });
        }
    }
}

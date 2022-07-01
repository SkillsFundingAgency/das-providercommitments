using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class InformViewModelMapper : IMapper<ChangeEmployerInformRequest, InformViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;

        public InformViewModelMapper(IOuterApiClient outerApiClient)
        {
            _outerApiClient = outerApiClient;
        }

        public async Task<InformViewModel> Map(ChangeEmployerInformRequest source)
        {
            var apiRequest = new GetInformRequest(source.ProviderId, source.ApprenticeshipId);
            var apiResponse = await _outerApiClient.Get<GetInformResponse>(apiRequest);

            return new InformViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeshipId = source.ApprenticeshipId,
                LegalEntityName = apiResponse.LegalEntityName
            };
        }
    }
}

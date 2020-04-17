using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class IChangeEmployerInformViewModelMapper : IMapper<InformRequest, IChangeEmployerInformViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public IChangeEmployerInformViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<IChangeEmployerInformViewModel> Map(InformRequest source)
        {
            var changeOfPartyRequest = await _commitmentsApiClient.GetChangeOfPartyRequests(source.ApprenticeshipId);

            if (changeOfPartyRequest.ChangeOfPartyRequests.Any(x =>
                x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer &&
                x.Status == ChangeOfPartyRequestStatus.Pending))
            {
                return new ChangeEmployerRequestDetailsViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ProviderId = source.ProviderId,
                    ApprenticeshipId = source.ApprenticeshipId
                };
            }
            else
            {
                return new InformViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ProviderId = source.ProviderId,
                    ApprenticeshipId = source.ApprenticeshipId
                };
            }
        }
    }
}

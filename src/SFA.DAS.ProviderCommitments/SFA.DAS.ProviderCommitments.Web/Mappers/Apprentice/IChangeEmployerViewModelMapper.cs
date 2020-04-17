using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class IChangeEmployerViewModelMapper : IMapper<ChangeEmployerRequest, IChangeEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public IChangeEmployerViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<IChangeEmployerViewModel> Map(ChangeEmployerRequest source)
        {
            var changeOfPartyRequest = await _commitmentsApiClient.GetChangeOfPartyRequests(source.ApprenticeshipId);

            if (changeOfPartyRequest.ChangeOfPartyRequests.Any(x =>
                x.OriginatingParty == Party.Provider
                && x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer &&
                (x.Status == ChangeOfPartyRequestStatus.Pending || x.Status == ChangeOfPartyRequestStatus.Approved)))
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

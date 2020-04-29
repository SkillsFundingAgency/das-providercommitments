using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
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
            var changeOfPartyRequest = (await _commitmentsApiClient.GetChangeOfPartyRequests(source.ApprenticeshipId))
                .ChangeOfPartyRequests.FirstOrDefault(x => x.OriginatingParty == Party.Provider
                                                    && x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer
                                                    && (x.Status == ChangeOfPartyRequestStatus.Pending || x.Status == ChangeOfPartyRequestStatus.Approved));

            if (changeOfPartyRequest != null)
            {
                var apprenticeDetails = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
                var priceEpisodes = await _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

                return new ChangeEmployerRequestDetailsViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ProviderId = source.ProviderId,
                    ApprenticeshipId = source.ApprenticeshipId,
                    Price = changeOfPartyRequest.Price,
                    StartDate = changeOfPartyRequest.StarDate,
                    EmployerName = changeOfPartyRequest.EmployerName,
                    CurrentEmployerName = apprenticeDetails.EmployerName,
                    CurrentStartDate = apprenticeDetails.StartDate,
                    CurrentPrice = priceEpisodes.PriceEpisodes.GetPrice(),
                    CohortId = changeOfPartyRequest.CohortId,
                    WithParty = changeOfPartyRequest.WithParty
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

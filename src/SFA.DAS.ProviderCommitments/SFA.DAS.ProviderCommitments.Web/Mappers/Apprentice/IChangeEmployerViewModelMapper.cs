using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class IChangeEmployerViewModelMapper : IMapper<ChangeEmployerRequest, IChangeEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;

        public IChangeEmployerViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        public async Task<IChangeEmployerViewModel> Map(ChangeEmployerRequest source)
        {
            var changeOfPartyRequest = (await _commitmentsApiClient.GetChangeOfPartyRequests(source.ApprenticeshipId))
                .ChangeOfPartyRequests.FirstOrDefault(x => x.OriginatingParty == Party.Provider
                                                    && x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer
                                                    && (x.Status == ChangeOfPartyRequestStatus.Pending || x.Status == ChangeOfPartyRequestStatus.Approved));

            var apprenticeDetails = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            if (changeOfPartyRequest != null)
            {
                var priceEpisodes = await _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

                var cohortReference = changeOfPartyRequest.CohortId.HasValue
                    ? _encodingService.Encode(changeOfPartyRequest.CohortId.Value, EncodingType.CohortReference)
                    : string.Empty;

                return new ChangeEmployerRequestDetailsViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ProviderId = source.ProviderId,
                    ApprenticeshipId = source.ApprenticeshipId,
                    Price = changeOfPartyRequest.Price,
                    StartDate = changeOfPartyRequest.StartDate,
                    EndDate = changeOfPartyRequest.EndDate,
                    EmployerName = changeOfPartyRequest.EmployerName,
                    CurrentEmployerName = apprenticeDetails.EmployerName,
                    CurrentStartDate = apprenticeDetails.StartDate,
                    CurrentEndDate = apprenticeDetails.EndDate,
                    CurrentPrice = priceEpisodes.PriceEpisodes.GetPrice(),
                    CohortId = changeOfPartyRequest.CohortId,
                    CohortReference = cohortReference,
                    WithParty = changeOfPartyRequest.WithParty,
                    Status = changeOfPartyRequest.Status,
                    EncodedNewApprenticeshipId = changeOfPartyRequest.NewApprenticeshipId.HasValue
                        ? _encodingService.Encode(changeOfPartyRequest.NewApprenticeshipId.Value,
                            EncodingType.ApprenticeshipId)
                        : string.Empty
                };
            }
            else
            {
                return new InformViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ProviderId = source.ProviderId,
                    ApprenticeshipId = source.ApprenticeshipId,
                    LegalEntityName = apprenticeDetails.EmployerName
                };
            }
        }
    }
}

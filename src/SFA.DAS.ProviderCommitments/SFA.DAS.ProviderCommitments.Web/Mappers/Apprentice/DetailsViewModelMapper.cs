using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(source.ApprenticeshipId);
            var pendingUpdatesTask = _commitmentApiClient.GetApprenticeshipUpdates(source.ApprenticeshipId, 
                new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest { Status = ApprenticeshipUpdateStatus.Pending });

            await Task.WhenAll(detailsResponseTask, priceEpisodesTask, pendingUpdatesTask);

            var detailsResponse = await detailsResponseTask;
            var priceEpisodes = await priceEpisodesTask;
            var pendingUpdates = await pendingUpdatesTask;   

            var pendingProviderUpdatesOnApprentice =
                pendingUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider);

            var allowEditApprentice =
                (detailsResponse.Status == ApprenticeshipStatus.Live ||
                detailsResponse.Status == ApprenticeshipStatus.WaitingToStart ||
                detailsResponse.Status == ApprenticeshipStatus.Paused) &&
                !pendingProviderUpdatesOnApprentice;

            return new DetailsViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeName = $"{detailsResponse.FirstName} {detailsResponse.LastName}",
                Employer = detailsResponse.EmployerName,
                Reference = _encodingService.Encode(detailsResponse.CohortId, EncodingType.CohortReference),
                Status = detailsResponse.Status,
                StopDate = detailsResponse.StopDate,
                AgreementId = _encodingService.Encode(detailsResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                DateOfBirth = detailsResponse.DateOfBirth,
                Uln = detailsResponse.Uln,
                CourseName = detailsResponse.CourseName,
                StartDate = detailsResponse.StartDate,
                EndDate = detailsResponse.EndDate,
                ProviderRef = detailsResponse.Reference,
                Cost = priceEpisodes.PriceEpisodes.GetPrice(),
                AllowEditApprentice = allowEditApprentice,
                HasPendingUpdate = pendingProviderUpdatesOnApprentice
            };
        }
    }
}
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly ILinkGenerator _linkGenerator;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService, ILinkGenerator linkGenerator)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _linkGenerator = linkGenerator;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodes = await _commitmentApiClient.GetPriceEpisodes(source.ApprenticeshipId);
            var detailsResponse = await detailsResponseTask;

            var allowEditApprentice =
                detailsResponse.Status == ApprenticeshipStatus.Live ||
                detailsResponse.Status == ApprenticeshipStatus.WaitingToStart ||
                detailsResponse.Status == ApprenticeshipStatus.Paused;

            var editApprenticeURL = allowEditApprentice
                ? _linkGenerator.ProviderApprenticeshipServiceLink($"{source.ProviderId}/apprentices/manage/{source.ApprenticeshipHashedId}/edit")
                : string.Empty;

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
                EditApprenticeURL = editApprenticeURL
            };
        }
    }
}
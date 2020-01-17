using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

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
            var detailsResponse = await _commitmentApiClient.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodes = await _commitmentApiClient.GetPriceEpisodes(source.ApprenticeshipId);

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
                CourseName = detailsResponse.CourseName
            };
        }
    }
}

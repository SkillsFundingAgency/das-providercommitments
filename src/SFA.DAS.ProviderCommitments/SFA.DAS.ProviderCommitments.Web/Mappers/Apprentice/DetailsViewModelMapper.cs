using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient)
        {
            _commitmentApiClient = commitmentApiClient;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var apiResponse = await _commitmentApiClient.GetApprenticeship(source.ApprenticeshipId);

            return new DetailsViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeName = $"{apiResponse.FirstName} {apiResponse.LastName}"
            };
        }
    }
}

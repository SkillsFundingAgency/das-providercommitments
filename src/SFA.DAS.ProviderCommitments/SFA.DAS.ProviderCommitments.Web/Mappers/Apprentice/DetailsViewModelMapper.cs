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

        public Task<DetailsViewModel> Map(DetailsRequest source)
        {
            return Task.FromResult(new DetailsViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                Name = "Apprenticeship Name"
            });
        }
    }
}

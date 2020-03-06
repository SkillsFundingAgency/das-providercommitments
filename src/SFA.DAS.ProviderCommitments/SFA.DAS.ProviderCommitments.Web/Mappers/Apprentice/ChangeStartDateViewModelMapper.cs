using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeStartDateViewModelMapper : IMapper<ChangeStartDateRequest, ChangeStartDateViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ChangeStartDateViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ChangeStartDateViewModel> Map(ChangeStartDateRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            return new ChangeStartDateViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StopDate = apprenticeship.StopDate,
                ProviderId = source.ProviderId
            };
        }
    }
}
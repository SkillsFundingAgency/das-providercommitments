using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DatesViewModelMapper : IMapper<DatesRequest, DatesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DatesViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<DatesViewModel> Map(DatesRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            return new DatesViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StopDate = apprenticeship.StopDate,
                ProviderId = source.ProviderId
            };
        }
    }
}
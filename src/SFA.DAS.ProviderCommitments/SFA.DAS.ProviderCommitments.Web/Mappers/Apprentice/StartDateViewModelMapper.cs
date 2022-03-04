using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class StartDateViewModelMapper : IMapper<StartDateRequest, StartDateViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public StartDateViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<StartDateViewModel> Map(StartDateRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            return new StartDateViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StopDate = apprenticeship.StopDate,
                EndDate = source.EndDate,
                StartDate = new MonthYearModel(source.StartDate),
                Price = source.Price,
                LegalEntityName = apprenticeship.EmployerName,
                DeliveryModel = apprenticeship.DeliveryModel,
            };
        }
    }
}
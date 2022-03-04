using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateViewModelMapper : IMapper<EndDateRequest, EndDateViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public EndDateViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<EndDateViewModel> Map(EndDateRequest source)
        {
            var ale = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);

            return new EndDateViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate,
                EmploymentEndDate = new MonthYearModel(source.EmploymentEndDate),
                EndDate = new MonthYearModel(source.EndDate),
                Price = source.Price,
                LegalEntityName = ale.LegalEntityName,
                DeliveryModel = source.DeliveryModel,
            };
        }
    }
}
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Requests.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ConfirmEmployerRequestToViewModelMapper : IMapper<ConfirmEmployerRequest, ConfirmEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ConfirmEmployerRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ConfirmEmployerViewModel> Map(ConfirmEmployerRequest source)
        {
            var accountLegalEntity = await _commitmentsApiClient.GetLegalEntity(source.AccountLegalEntityId);

            return new ConfirmEmployerViewModel
            {
                EmployerAccountName = accountLegalEntity.AccountName,
                EmployerAccountLegalEntityName = accountLegalEntity.LegalEntityName,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            };
        }
    }
}

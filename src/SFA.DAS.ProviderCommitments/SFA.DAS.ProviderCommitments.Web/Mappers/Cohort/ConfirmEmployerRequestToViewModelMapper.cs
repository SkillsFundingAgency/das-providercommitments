using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class ConfirmEmployerRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
    : IMapper<ConfirmEmployerRequest, ConfirmEmployerViewModel>
{
    public async Task<ConfirmEmployerViewModel> Map(ConfirmEmployerRequest source)
    {
        var accountLegalEntity = await commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);

        return new ConfirmEmployerViewModel
        {
            EmployerAccountName = accountLegalEntity.AccountName,
            EmployerAccountLegalEntityName = accountLegalEntity.LegalEntityName,
            ProviderId = source.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId
        };
    }
}
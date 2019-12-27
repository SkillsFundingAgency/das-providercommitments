using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderUrlHelper;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ConfirmEmployerRequestToViewModelMapper : IMapper<ConfirmEmployerRequest, ConfirmEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ILinkGenerator _linkGenerator;

        public ConfirmEmployerRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ILinkGenerator linkGenerator)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _linkGenerator = linkGenerator;
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
                BackLink = _linkGenerator.ProviderApprenticeshipServiceLink("account")
            };
        }
    }
}

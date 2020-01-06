using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ConfirmEmployerViewModelToCreateEmptyCohortRequestMapper : IMapper<ConfirmEmployerViewModel, CreateEmptyCohortRequest>
    {
        public Task<CreateEmptyCohortRequest> Map(ConfirmEmployerViewModel source)
        {
            return Task.FromResult(new CreateEmptyCohortRequest
            {
                AccountLegalEntityId = source.AccountLegalEntityId.Value,
                ProviderId = source.ProviderId,
            });
        }
    }
}

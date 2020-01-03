using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using WebApp= SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using API=SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateEmptyCohortRequestToApiCreateEmptyCohortRequestMapper : IMapper<CreateEmptyCohortRequest, API.CreateEmptyCohortRequest>
    {
        public Task<API.CreateEmptyCohortRequest> Map(CreateEmptyCohortRequest source)
        {
            return Task.FromResult(new API.CreateEmptyCohortRequest
            {
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId
            });
        }
    }
}

using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SendCohortRequestMapper : IMapper<DetailsViewModel, SendCohortRequest>
    {
        public Task<SendCohortRequest> Map(DetailsViewModel source)
        {
            return Task.FromResult(new SendCohortRequest
            {
                Message = source.SendMessage
            });
        }
    }
}

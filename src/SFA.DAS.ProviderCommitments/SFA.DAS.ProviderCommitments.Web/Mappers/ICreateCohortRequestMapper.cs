using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface ICreateCohortRequestMapper
    {
        Task<CreateCohortRequest> MapAsync(AddDraftApprenticeshipViewModel source);
    }
}
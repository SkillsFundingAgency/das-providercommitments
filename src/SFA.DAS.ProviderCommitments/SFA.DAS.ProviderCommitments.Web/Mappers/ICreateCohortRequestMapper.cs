using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface ICreateCohortRequestMapper
    {
        CreateCohortRequest Map(AddDraftApprenticeshipViewModel source);
    }
}
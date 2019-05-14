using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Models.ApiModels;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface IAddDraftApprenticeshipRequestMapper
    {
        AddDraftApprenticeshipToCohortRequest Map(AddDraftApprenticeshipViewModel source);
    }
}
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class CreateCohortResponseToCreateOverlappingTrainingDateRequestMapper : IMapper<CreateCohortResponse, CreateOverlappingTrainingDateApimRequest>
    {
        public Task<CreateOverlappingTrainingDateApimRequest> Map(CreateCohortResponse source)
        {
            return Task.FromResult(new CreateOverlappingTrainingDateApimRequest
            {
                DraftApprenticeshipId = source.DraftApprenticeshipId.Value
            });
        }
    }
}

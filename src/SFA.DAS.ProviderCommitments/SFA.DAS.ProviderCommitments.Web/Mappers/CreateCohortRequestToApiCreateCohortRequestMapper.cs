using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using WebApp= SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using API=SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestToApiCreateCohortRequestMapper : IMapper<WebApp.CreateCohortRequest, API.CreateCohortRequest>
    {
        public Task<API.CreateCohortRequest> Map(WebApp.CreateCohortRequest source)
        {
            return Task.FromResult(new API.CreateCohortRequest
            {
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                Uln = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                OriginatorReference = source.OriginatorReference,
                ReservationId = source.ReservationId
            });
        }
    }
}

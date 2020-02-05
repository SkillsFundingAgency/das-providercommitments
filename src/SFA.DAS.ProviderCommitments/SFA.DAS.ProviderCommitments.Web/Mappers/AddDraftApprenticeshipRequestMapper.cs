using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipRequestMapper : IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest>
    {
        public Task<AddDraftApprenticeshipRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            return Task.FromResult(new AddDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference
            });
        }
    }
}

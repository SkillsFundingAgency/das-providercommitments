using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Models.ApiModels;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipToCohortRequestMapper : IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest>
    {
        public AddDraftApprenticeshipToCohortRequest Map(AddDraftApprenticeshipViewModel source)
        {
            return new AddDraftApprenticeshipToCohortRequest
            {
                CohortId = source.Cohort.CohortId.Value,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth.Date,
                UniqueLearnerNumber = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference
            };
        }
    }
}

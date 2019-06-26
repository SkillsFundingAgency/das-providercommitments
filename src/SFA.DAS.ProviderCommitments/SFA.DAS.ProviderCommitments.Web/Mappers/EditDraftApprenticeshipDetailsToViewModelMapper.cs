using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipDetailsToViewModelMapper : IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel>
    {
        public EditDraftApprenticeshipViewModel Map(EditDraftApprenticeshipDetails source) =>
            new EditDraftApprenticeshipViewModel(source.DateOfBirth, source.StartDate, source.EndDate)
            {
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Uln = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                Reference = source.OriginatorReference
            };
    }
}

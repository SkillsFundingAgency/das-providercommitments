using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestMapper : ICreateCohortRequestMapper
    {
        public CreateCohortRequest Map(AddDraftApprenticeshipViewModel source)
        {
            return new CreateCohortRequest
            {
                EmployerAccountId = source.EmployerAccountId,
                LegalEntityId = source.LegalEntityId,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.BirthDate.Date,
                UniqueLearnerNumber = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.FinishDate.Date,
                OriginatorReference = source.Reference
            };
        }
    }
}

using SFA.DAS.HashingService;
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
                AccountLegalEntityId = source.AccountLegalEntity.AccountLegalEntityId ?? 0,
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

using System;
using MediatR;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortRequest : IRequest<CreateCohortResponse>
    {
        public long EmployerAccountId { get; set; }
        public string LegalEntityId { get; set; }
        public int ProviderId { get; set; }
        public Guid ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string UniqueLearnerNumber { get; set; }
        public string CourseCode { get; set; }
        public int? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OriginatorReference { get; set; }
    }
}

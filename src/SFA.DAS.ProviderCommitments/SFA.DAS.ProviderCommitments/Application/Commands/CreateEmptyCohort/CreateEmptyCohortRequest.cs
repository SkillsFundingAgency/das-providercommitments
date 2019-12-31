using MediatR;
using System;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort
{
    public class CreateEmptyCohortRequest : IRequest<CreateEmptyCohortResponse>
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long ProviderId { get; set; }
    }
}

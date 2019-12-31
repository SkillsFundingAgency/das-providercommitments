using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort
{
    public class CreateEmptyCohortValidator : AbstractValidator<CreateEmptyCohortRequest>
    {
        public CreateEmptyCohortValidator()
        {
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.AccountLegalEntityId).NotEmpty();
        }
    }
}

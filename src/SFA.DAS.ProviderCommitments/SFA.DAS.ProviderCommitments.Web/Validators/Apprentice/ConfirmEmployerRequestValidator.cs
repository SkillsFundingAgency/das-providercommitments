﻿using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ConfirmEmployerRequestValidator : AbstractValidator<ConfirmEmployerRequest>
    {
        public ConfirmEmployerRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(x => x.AccountLegalEntityId).GreaterThan(0);
        }
    }
}

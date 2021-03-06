﻿using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class ConfirmEmployerViewModelValidator : AbstractValidator<ConfirmEmployerViewModel>
    {
        public ConfirmEmployerViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.Confirm).NotNull().WithMessage("Please select an option");
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(x => x.AccountLegalEntityId).GreaterThan(0);
        }
    }
}

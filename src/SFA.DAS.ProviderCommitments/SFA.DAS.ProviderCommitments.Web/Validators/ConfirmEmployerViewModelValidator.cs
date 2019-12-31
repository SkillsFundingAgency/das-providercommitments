using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ConfirmEmployerViewModelValidator : AbstractValidator<ConfirmEmployerViewModel>
    {
        public ConfirmEmployerViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.Confirm).NotNull().WithMessage("Please select an option");
        }
    }
}

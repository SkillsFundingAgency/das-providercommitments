using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ConfirmEditApprenticeshipViewModelValidator : AbstractValidator<ConfirmEditApprenticeshipViewModel>
    {
        public ConfirmEditApprenticeshipViewModelValidator()
        {
            RuleFor(r => r.ConfirmChanges).NotNull()
                .WithMessage("Select an option");
        }
    }
}

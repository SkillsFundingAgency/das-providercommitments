using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class FileDiscardViewModelValidator : AbstractValidator<FileDiscardViewModel>
    {
        public FileDiscardViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);            
            RuleFor(x => x.FileDiscardConfirmed).NotNull().WithMessage(" Confirm you want to discard this file");
        }
    }
}

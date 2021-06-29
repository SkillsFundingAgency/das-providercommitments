using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DeleteConfirmationViewModelValidator : AbstractValidator<DeleteConfirmationViewModel>
    {
        public DeleteConfirmationViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.CohortReference).NotEmpty();
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Confirm if you would like to delete this apprentice");
        }
    }
}

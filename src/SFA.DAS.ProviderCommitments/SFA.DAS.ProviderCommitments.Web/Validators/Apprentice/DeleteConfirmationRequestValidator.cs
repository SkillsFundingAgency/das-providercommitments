using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class DeleteConfirmationRequestValidator : AbstractValidator<DeleteConfirmationRequest>
    {
        public DeleteConfirmationRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.DraftApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.CohortReference).NotEmpty();            
        }
    }
}

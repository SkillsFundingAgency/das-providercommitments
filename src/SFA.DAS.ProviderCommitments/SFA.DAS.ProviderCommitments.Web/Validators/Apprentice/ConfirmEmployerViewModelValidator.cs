using FluentValidation;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class ConfirmEmployerViewModelValidator : AbstractValidator<ConfirmEmployerViewModel>
    {
        public ConfirmEmployerViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.Confirm).NotNull().WithMessage("Please select an option");
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();

            RuleFor(x => x.Confirm)
                .Must((x,y) => !x.IsFlexiJobAgency)
                .When(x => x.DeliveryModel == DeliveryModel.PortableFlexiJob)
                .When(x => x.Confirm == true)
                .WithMessage("Apprentices on the Portable Flexi-Job apprenticeship delivery model cannot change to a Flexi-Job Apprenticeship Agency employer.");
        }
    }
}

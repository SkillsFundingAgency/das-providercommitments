using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class SelectDeliveryModelViewModelValidator : AbstractValidator<SelectDeliveryModelViewModel>
    {
        public SelectDeliveryModelViewModelValidator()
        {
            RuleFor(x => x.DeliveryModel).NotNull().WithMessage("You must select the apprenticeship delivery model");
        }
    }
}

using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators.DraftApprenticeship
{
    public class SelectDeliveryModelForEditViewModelValidator : AbstractValidator<SelectDeliveryModelForEditViewModel>
    {
        public SelectDeliveryModelForEditViewModelValidator()
        {
            RuleFor(x => x.DeliveryModel).NotEmpty().WithMessage("You must select the apprenticeship delivery model");
        }
    }
}

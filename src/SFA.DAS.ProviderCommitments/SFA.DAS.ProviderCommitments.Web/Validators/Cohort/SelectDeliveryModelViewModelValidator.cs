using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectDeliveryModelViewModelValidator : AbstractValidator<SelectDeliveryModelViewModel>
    {
        public SelectDeliveryModelViewModelValidator()
        {
            RuleFor(x => x.DeliveryModel).NotEmpty().WithMessage("You must select the apprenticeship delivery model");
        }
    }
}

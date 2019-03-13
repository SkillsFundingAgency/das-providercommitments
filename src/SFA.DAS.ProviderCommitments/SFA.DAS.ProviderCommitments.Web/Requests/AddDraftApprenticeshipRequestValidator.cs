using FluentValidation;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class AddDraftApprenticeshipRequestValidator : AbstractValidator<AddDraftApprenticeshipRequest>
    {
        public AddDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ReservationId).NotEmpty();
            //RuleFor(model => model.Account).NotEmpty();
            //RuleFor(model => model.Account.AccountId).GreaterThanOrEqualTo(0L);
            //RuleFor(model => model.AccountLegalEntity).NotEmpty().When(model => model != null).
            //DependentRules(accountLegalEntity => accountLegalEntityId.).GreaterThanOrEqualTo(0L);
            RuleFor(model => model.StartMonthYear)
                .Must(monthYear => new MonthYearModel(monthYear).IsValid)
                .When(model => !string.IsNullOrWhiteSpace(model.StartMonthYear))
                .WithMessage("{PropertyName} is invalid");
        }
    }
}

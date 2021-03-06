﻿using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class CreateCohortWithDraftApprenticeshipRequestValidator : AbstractValidator<CreateCohortWithDraftApprenticeshipRequest>
    {
        public CreateCohortWithDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ReservationId).NotEmpty();
            RuleFor(model => model.StartMonthYear)
                .Must(monthYear => new MonthYearModel(monthYear).IsValid)
                .When(model => !string.IsNullOrWhiteSpace(model.StartMonthYear))
                .WithMessage("{PropertyName} is invalid");
        }
    }
}

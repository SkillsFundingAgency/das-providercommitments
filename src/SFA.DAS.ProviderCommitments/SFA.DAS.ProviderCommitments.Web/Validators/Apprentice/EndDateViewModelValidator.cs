﻿using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class EndDateViewModelValidator : AbstractValidator<EndDateViewModel>
    {
        public EndDateViewModelValidator()
        {
            RuleFor(x => x.ApprenticeshipHashedId)
                .NotEmpty();
            RuleFor(x => x.EmployerAccountLegalEntityPublicHashedId)
                .NotEmpty();
            RuleFor(x => x.ProviderId)
                .GreaterThan(0);
            RuleFor(x => x.AccountLegalEntityId)
                .GreaterThan(0);
            RuleFor(x => x.StartDate)
                .Must(field => field.IsValidMonthYear());
            RuleFor(x => x.EndDate)
                .Must(y => y.IsValid)
                .WithMessage("The end date is not valid")
                .When(z => z.EndDate.HasValue);


            When(x => x.DeliveryModel != DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must(y => y.HasValue)
                    .WithMessage("Enter a date after the new training start date");

                RuleFor(x => x.EndDate)
                    .Must((y, _) => y.EndDate.Date > (new MonthYearModel(y.StartDate).Date))
                    .WithMessage("Enter a date after the new training start date")
                    .When(a => a.EndDate.IsValid);

            });

            When(x => x.DeliveryModel == DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must(y => y.HasValue)
                    .WithMessage("You must enter the projected apprenticeship training end date");

                RuleFor(x => x.EndDate)
                    .Must((y, _) => y.EndDate.Date > (new MonthYearModel(y.StartDate).Date))
                    .WithMessage("This date must be later than the employment start date")
                    .When(a => a.EndDate.IsValid);

                RuleFor(x => x.EmploymentEndDate)
                    .Must(x => x.HasValue)
                    .WithMessage("You must enter the end date for this employment");

                RuleFor(x => x.EmploymentEndDate)
                    .Must((model, date) => date.Date <= model.EndDate.Date)
                    .WithMessage("This date must not be later than the projected apprenticeship training end date")
                    .When(x => x.EmploymentEndDate.IsValid && x.EndDate.IsValid);

                RuleFor(x => x.EndDate)
                    .Must((model, date) => date.Date >= model.EmploymentEndDate.Date)
                    .WithMessage("This date must not be before the end date for this employment")
                    .When(x => x.EmploymentEndDate.IsValid && x.EndDate.IsValid);

                RuleFor(x => x.EmploymentEndDate)
                    .Must((model, date) => date.Date.Value >= model.StartDateTime.Date.AddMonths(3))
                    .WithMessage("This date must be at least 3 months later than the employment start date")
                    .When(x => x.EmploymentEndDate.IsValid);
            });
        }
    }
}
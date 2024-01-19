using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class TrainingDatesViewModelValidator : AbstractValidator<TrainingDatesViewModel>
    {
        private IAcademicYearDateProvider _academicYearDateProvider;

        public TrainingDatesViewModelValidator(IAcademicYearDateProvider academicYearDateProvider)
        {
            _academicYearDateProvider = academicYearDateProvider;

            RuleFor(x => x.ApprenticeshipHashedId)
                .NotEmpty();
            RuleFor(x => x.ProviderId)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .Must((y, _) => y.StartDate.Date < y.EndDate.Date)
                .WithMessage("Enter a start date prior to the new training end date")
                .When(a => a.EndDate.HasValue && a.EndDate.IsValid && a.StartDate.HasValue && a.StartDate.IsValid);
            
            RuleFor(x => x.StartDate)
                .Must((y, _) => y.StartDate.Date > y.CurrentStartDate)
                .WithMessage("The new training start date cannot be before the training start date on the existing record");

            RuleFor(x => x.StartDate)
                .Must((y, _) =>
                    y.StartDate.Date <= (new MonthYearModel(_academicYearDateProvider.CurrentAcademicYearEndDate
                        .AddYears(1).ToString("MMyyyy")).Date))
                .WithMessage("The start date must be no later than one year after the end of the current teaching year")
                .When(a => a.StartDate.HasValue && a.StartDate.IsValid)
                .Unless(a => a.EndDate.HasValue && a.EndDate.IsValid && a.StartDate.Date > a.EndDate.Date);

            RuleFor(x => x.StartDate)
                .Must(y => y.IsValid)
                .WithMessage("You must enter a valid date, for example 09 2022");

            RuleFor(x => x.EndDate)
                .Must(y => y.IsValid)
                .WithMessage("You must enter a valid date, for example 09 2022")
                .When(z => z.EndDate.HasValue);

            When(x => x.DeliveryModel != DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must(y => y.HasValue)
                    .WithMessage("Enter a date after the new training start date");

                RuleFor(x => x.EndDate)
                    .Must((y, _) => y.EndDate.Date > y.StartDate.Date)
                    .WithMessage("Enter a date after the new training start date")
                    .When(a => a.EndDate.IsValid);
            });

            When(x => x.DeliveryModel == DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must(y => y.IsValid)
                    .WithMessage("You must enter a valid date, for example 09 2022");

                RuleFor(x => x.EndDate)
                    .Must((y, _) => y.EndDate.Date > (y.StartDate.Date))
                    .WithMessage("This date must be later than the employment start date")
                    .When(a => a.EndDate.IsValid && a.EmploymentEndDate.IsValid &&
                               a.EmploymentEndDate.Date <= a.EndDate.Date);

                RuleFor(x => x.EmploymentEndDate)
                    .Must(x => x.IsValid)
                    .WithMessage("You must enter a valid date, for example 09 2022");

                RuleFor(x => x.EmploymentEndDate)
                    .Must((model, _) => model.EmploymentEndDate.Date <= model.EndDate.Date)
                    .WithMessage("This date must not be later than the projected apprenticeship training end date")
                    .When(x => x.EmploymentEndDate.IsValid && x.EndDate.IsValid);

                RuleFor(x => x.EndDate)
                    .Must((model, _) => model.EmploymentEndDate.Date <= model.EndDate.Date)
                    .WithMessage("This date must not be before the end date for this employment")
                    .When(x => x.EmploymentEndDate.IsValid && x.EndDate.IsValid);

                RuleFor(x => x.EmploymentEndDate)
                    .Must((model, _) => model.EmploymentEndDate.Date >= model.StartDate.Date.Value.AddMonths(3))
                    .WithMessage("This date must be at least 3 months later than the employment start date")
                    .When(x => x.EmploymentEndDate.IsValid && x.StartDate.Date.HasValue);
            });
        }
    }
}
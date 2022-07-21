﻿using FluentValidation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class StartDateViewModelValidator : AbstractValidator<StartDateViewModel>
    {
        private IAcademicYearDateProvider _academicYearDateProvider;

        public StartDateViewModelValidator(IAcademicYearDateProvider academicYearDateProvider)
        {
            _academicYearDateProvider = academicYearDateProvider;

            RuleFor(x => x.ApprenticeshipHashedId)
                .NotEmpty();
            RuleFor(x => x.ProviderId)
                .GreaterThan(0);
            RuleFor(x => x.StopDate)
                .NotEmpty();
            RuleFor(x => x.StartDate)
                .Must((y, _) => y.StartDate.Date < (new MonthYearModel(y.EndDate).Date))
                .WithMessage("Enter a start date prior to the new training end date")
                .When(a => a.EndDate != null && a.StartDate.HasValue && a.StartDate.IsValid);
            RuleFor(x => x.StartDate)
                .Must((y, _) => y.StartDate.Date <= (new MonthYearModel(_academicYearDateProvider.CurrentAcademicYearEndDate.AddYears(1).ToString("MMyyyy")).Date))
                .WithMessage("The start date must be no later than one year after the end of the current teaching year")
                .When(a => a.StartDate.HasValue && a.StartDate.IsValid)
                .Unless(a => a.EndDate != null && a.StartDate.Date > new MonthYearModel(a.EndDate).Date);
            RuleFor(x => x.StartDate)
                .Must(y => y.IsValid)
                .WithMessage("You must enter a valid date, for example 09 2022");

            When(x => x.DeliveryModel != DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.StartDate)
                    .Must((y, _) => y.StartDate.Date >= y.StopDate)
                    .WithMessage("The new training start date cannot be before the stop date")
                    .When(a => a.StartDate.HasValue && a.StartDate.IsValid);
            });

            When(x => x.DeliveryModel == DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.StartDate)
                    .Must((y, _) => y.StartDate.Date >= y.StopDate)
                    .WithMessage("This date must not be before the previous employment end date")
                    .When(a => a.StartDate.HasValue && a.StartDate.IsValid);
            });
        }
    }
}
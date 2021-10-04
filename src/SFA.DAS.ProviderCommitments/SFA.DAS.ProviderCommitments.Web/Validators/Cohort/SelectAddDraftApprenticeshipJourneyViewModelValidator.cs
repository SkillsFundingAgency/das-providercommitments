﻿using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyViewModelValidator : AbstractValidator<SelectAddDraftApprenticeshipJourneyViewModel>
    {
        public SelectAddDraftApprenticeshipJourneyViewModelValidator()
        {
            RuleFor(x => x.Selection).NotNull().WithMessage("You need to select whether you want add to an existing cohort or create a new one");
        }
    }
}
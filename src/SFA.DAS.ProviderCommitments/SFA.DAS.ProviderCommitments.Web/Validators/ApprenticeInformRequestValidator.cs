﻿using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ApprenticeInformRequestValidator : AbstractValidator<ChangeEmployerRequest>
    {
        public ApprenticeInformRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
        }
    }
}

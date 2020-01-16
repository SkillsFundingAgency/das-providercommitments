using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ApprenticeDetailsRequestValidator : AbstractValidator<ApprenticeDetailsRequest>
    {
        public ApprenticeDetailsRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
        }
    }
}

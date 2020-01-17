using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class SelectEmployerRequestValidator : AbstractValidator<SelectEmployerRequest>
    {
        public SelectEmployerRequestValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
        }
    }
}

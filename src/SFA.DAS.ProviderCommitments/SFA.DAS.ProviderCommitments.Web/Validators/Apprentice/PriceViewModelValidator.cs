﻿using FluentValidation;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Apprentice
{
    public class PriceViewModelValidator : AbstractValidator<PriceViewModel>
    {
        public PriceViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipHashedId).NotEmpty();
            RuleFor(x => x.Price).NotEmpty().WithMessage("You must enter a valid price. For example, for £1,000 enter 1000");

            When(x => x.DeliveryModel != DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.Price).GreaterThanOrEqualTo(1).WithMessage("Enter the new agreed apprenticeship price");
                RuleFor(x => x.Price)
                    .LessThanOrEqualTo(100000)
                    .WithMessage("The new agreed apprenticeship price must be £100,000 or less")
                    .When(x => x.Price >= 1);
            });

            When(x => x.DeliveryModel == DeliveryModel.PortableFlexiJob, () =>
            {
                RuleFor(x => x.Price).GreaterThanOrEqualTo(1).WithMessage("The price must be greater than zero");
                RuleFor(x => x.Price).LessThanOrEqualTo(100000).WithMessage("The total agreed apprenticeship price must be £100,000 or less");
                
                RuleFor(x => x.EmploymentPrice).NotEmpty().WithMessage("You must enter a valid price. For example, for £1,000 enter 1000");
                RuleFor(x => x.EmploymentPrice).GreaterThanOrEqualTo(1).WithMessage("The price must be greater than zero");
                RuleFor(x => x.EmploymentPrice).LessThanOrEqualTo(x => x.Price)
                    .WithMessage("This price must not be more than than the total agreed apprenticeship price")
                    .When(x => x.EmploymentPrice > 1 && x.Price > 1);
            });
        }
    }
}

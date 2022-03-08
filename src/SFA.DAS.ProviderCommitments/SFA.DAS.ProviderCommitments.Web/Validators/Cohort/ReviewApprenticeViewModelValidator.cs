using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class ReviewApprenticeViewModelValidator : AbstractValidator<FileUploadReviewApprenticeViewModel>
    {
        public ReviewApprenticeViewModelValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.CohortRef).NotNull();            
        }
    }
}


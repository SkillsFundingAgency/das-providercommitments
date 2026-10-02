using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Models.Learners;

public class SelectMultipleLearnerRecordsValidationErrorsViewModel
{
    public long ProviderId { get; set; }
    public Guid? CacheKey { get; set; }
    public IEnumerable<LearnerDataValidationError> ValidationErrors { get; set; } = new List<LearnerDataValidationError>();
}
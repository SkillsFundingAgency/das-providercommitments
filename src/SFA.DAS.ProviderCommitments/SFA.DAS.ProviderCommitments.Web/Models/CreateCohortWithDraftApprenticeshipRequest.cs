using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models;

public class CreateCohortWithDraftApprenticeshipRequest : BaseCreateCohortWithDraftApprenticeshipRequest, IAuthorizationContextModel
{
    public long AccountLegalEntityId { get; set; }
}

public class BaseCreateCohortWithDraftApprenticeshipRequest
{
    public Guid CacheKey { get; set; }
    public long ProviderId { get; set; }
    public Guid? ReservationId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public string StartMonthYear { get; set; }
    public string CourseCode { get; set; }
    public DeliveryModel? DeliveryModel { get; set; }
    public bool? IsOnFlexiPaymentPilot { get; set; }

    public BaseCreateCohortWithDraftApprenticeshipRequest CloneBaseValues()
    {
        return this.ExplicitClone();
    }
}
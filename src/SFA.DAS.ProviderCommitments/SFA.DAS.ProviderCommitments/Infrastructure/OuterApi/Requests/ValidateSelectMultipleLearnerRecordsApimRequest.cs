using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

public class ValidateSelectMultipleLearnerRecordsApimRequest : ApimSaveDataRequest
{
    public long ProviderId { get; set; }
    public IEnumerable<long> LearnerIds { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string AgreementId { get; set; }
    public long AccountId { get; set; }
    public bool IsLevyEmployer { get; set; }
    public IEnumerable<SelectMultipleReservationAssignment> ReservationAssignments { get; set; }
}

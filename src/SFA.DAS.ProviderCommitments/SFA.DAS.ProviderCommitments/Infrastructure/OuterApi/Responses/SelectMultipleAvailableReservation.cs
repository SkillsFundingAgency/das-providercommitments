using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

public class SelectMultipleAvailableReservation
{
    public Guid Id { get; set; }
    public long AccountLegalEntityId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

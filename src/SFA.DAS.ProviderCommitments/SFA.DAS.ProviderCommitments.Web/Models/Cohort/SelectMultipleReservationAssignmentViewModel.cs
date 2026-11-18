using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleReservationAssignmentViewModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public int RemainingNewReservations { get; set; }
    public List<SelectMultipleReservationAssignmentRow> Assignments { get; set; } = new();
}

public class SelectMultipleReservationAssignmentRow
{
    public long LearnerId { get; set; }
    public string LearnerName { get; set; }
    public DateTime StartDate { get; set; }
    public Guid? ReservationId { get; set; }
    public List<SelectMultipleReservationOption> Options { get; set; } = new();
}

public class SelectMultipleReservationOption
{
    public Guid? ReservationId { get; set; }
    public string Text { get; set; }
    public bool IsNew { get; set; }
}

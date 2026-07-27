using System.ComponentModel;

namespace SFA.DAS.ProviderCommitments.Enums;

public enum LearningChangeType : byte
{
    [Description("Auto approved")]
    AutoApproved = 0,

    [Description("Declined")]
    Rejected = 1,

    [Description("Approved")]
    EmployerApproved = 2,

    [Description("Declined")]
    EmployerRejected = 3,

    [Description("Manual update")]
    ManualUpdate = 4
}
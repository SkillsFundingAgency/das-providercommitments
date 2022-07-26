using System.ComponentModel;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types
{
    public enum ApprenticeshipStatus : short
    {
        [Description("Waiting to start")]
        WaitingToStart = 0,
        [Description("Live")]
        Live = 1,
        [Description("Paused")]
        Paused = 2,
        [Description("Stopped")]
        Stopped = 3,
        [Description("Completed")]
        Completed = 4,
        [Description("Unknown")]
        Unknown
    }
}

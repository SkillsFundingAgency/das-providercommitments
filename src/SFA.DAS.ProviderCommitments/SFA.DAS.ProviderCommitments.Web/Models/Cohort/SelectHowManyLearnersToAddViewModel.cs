namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectHowManyLearnersToAddViewModel
{
    public long ProviderId { get; set; }

    public HowManyLearnersToAddOptions? Selection { get; set; }
}

public enum HowManyLearnersToAddOptions
{
    Single,
    Multiple
}
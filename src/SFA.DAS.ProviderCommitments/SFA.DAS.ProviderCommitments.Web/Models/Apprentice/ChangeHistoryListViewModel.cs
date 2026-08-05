using SFA.DAS.ProviderCommitments.Enums;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class ChangeHistoryListViewModel
{
    public List<ChangeHistoryViewModel> ChangeHistory { get; set; } = [];

    public string Name { get; set; }

    public string ApprenticeshipHashedId { get; set; }

    public long ProviderId { get; set; }

    public DateTime AvailableFrom { get; set; }
}

public class ChangeHistoryViewModel
{
    public DateTime AppliedDate { get; set; }

    public string Description { get; set; }

    public LearningChangeType ChangeType { get; set; }

    public Guid Id { get; set; }
}
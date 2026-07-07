using SFA.DAS.ProviderCommitments.Enums;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class GetAllChangeHistoryListViewModel
{
    public List<GetAllChangeHistoryViewModel> ChangeHistory { get; set; } = [];
    public long ProviderId { get; set; }

    public DateTime AvailableFrom { get; set; }
}

public class GetAllChangeHistoryViewModel
{
    public DateTime AppliedDate { get; set; }

    public string Description { get; set; }

    public LearningChangeType ChangeType { get; set; }

    public Guid Id { get; set; }
    public string LearnerName { get; set; }
    public string EmployerName { get; set; }
}
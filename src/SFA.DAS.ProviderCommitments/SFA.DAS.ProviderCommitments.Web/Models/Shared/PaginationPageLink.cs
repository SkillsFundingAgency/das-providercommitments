namespace SFA.DAS.ProviderCommitments.Web.Models.Shared;

public class PaginationPageLink
{
    public string Label { get; set; }
    public string AriaLabel { get; set; }
    public bool? IsCurrent { get; set; }
    public Dictionary<string, string> RouteData { get; set; }
}

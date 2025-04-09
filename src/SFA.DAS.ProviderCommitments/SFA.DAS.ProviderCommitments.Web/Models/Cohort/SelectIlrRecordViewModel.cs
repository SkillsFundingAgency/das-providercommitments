using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectIlrRecordViewModel : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountName { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string EmployerAccountLegalEntityName { get; set; }
    public List<IlrApprenticeshipSummary> IlrApprenticeships { get; set; } = new();
    public string PageTitle => $"Select apprentice from ILR for {EmployerAccountName}";
    public string MatchingRecordCount { get; set; }

    public string SortedByHeaderClassName { get; set; }
    public const string HeaderClassName = "das-table__sort";

    public void SortedByHeader()
    {
        SortedByHeaderClassName += HeaderClassName;
        if (FilterModel.ReverseSort)
        {
            SortedByHeaderClassName += " das-table__sort--desc";
        }
        else
        {
            SortedByHeaderClassName += " das-table__sort--asc";
        }
    }

    public DateTime? LastIlrSubmittedOn { get; set; } = DateTime.Now;

    public string LastIlrSubmittedOnDesc
    {
        get
        {
            if (LastIlrSubmittedOn.HasValue == false)
            {
                return "";
            }

            return $"Last updated {LastIlrSubmittedOn.Value:h:mmtt} on {LastIlrSubmittedOn.Value:dddd d MMMM}";
        }
    }

    public IlrRecordsFilterModel FilterModel { get; set; }

}

public class IlrRecordsFilterModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public int PageNumber { get; set; } = 1;
    public string SearchTerm { get; set; }
    public int TotalNumberOfApprenticeships { get; set; }
    public int TotalNumberOfApprenticeshipsFound { get; set; }
    public string TotalNumberOfApprenticeshipsFoundDescription => TotalNumberOfApprenticeshipsFound == 1
        ? "1 apprentice record"
        : $"{TotalNumberOfApprenticeshipsFound} apprentice records";

    public bool ShowSearch => TotalNumberOfApprenticeships >= Constants.IlrRecordSearch.NumberOfApprenticesRequiredForSearch;
    public string SortField { get; set; }
    public bool ReverseSort { get; set; }

    public Dictionary<string, string> RouteData => BuildRouteData();

    private Dictionary<string, string> BuildRouteData()
    {
        var routeData = new Dictionary<string, string>
        {
            {nameof(ProviderId), ProviderId.ToString()},
            {nameof(EmployerAccountLegalEntityPublicHashedId), EmployerAccountLegalEntityPublicHashedId}
        };

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            routeData.Add(nameof(SearchTerm), SearchTerm);
        }

        return routeData;
    }

    //private Dictionary<string, string> BuildPagedRouteData(int pageNumber)
    //{
    //    var routeData = BuildRouteData();

    //    routeData.Add(nameof(PageNumber), pageNumber.ToString());

    //    if (!string.IsNullOrEmpty(SortField))
    //    {
    //        routeData.Add(nameof(SortField), SortField);

    //        routeData.Add(nameof(ReverseSort), ReverseSort.ToString());
    //    }

    //    return routeData;
    //}

    public Dictionary<string, string> BuildSortRouteData(string sortField)
    {
        var routeData = BuildRouteData();

        var reverseSort = !string.IsNullOrEmpty(SortField)
                          && SortField.ToLower() == sortField.ToLower()
                          && !ReverseSort;
        routeData.Add(nameof(ReverseSort), reverseSort.ToString());
        routeData.Add(nameof(SortField), sortField);

        return routeData;
    }




}

public class IlrApprenticeshipSummary
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name => $"{FirstName} {LastName}";
    public long Uln { get; set; }
    public string CourseName { get; set; }
}
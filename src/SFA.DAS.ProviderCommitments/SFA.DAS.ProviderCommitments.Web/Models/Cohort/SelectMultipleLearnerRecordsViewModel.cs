using FluentValidation.Results;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleLearnerRecordsViewModel : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountName { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string CohortReference { get; set; }
    public long? CohortId { get; set; }
    public Guid? CacheKey { get; set; }
    public Guid? ReservationId { get; set; }

    public List<LearnerSummary> Learners { get; set; } = new();
    public List<long> SelectedLearnersIds { get; set; } = new List<long>();
    public List<LearnerSummary> SelectedLearners => Learners.Where(l => SelectedLearnersIds.Contains(l.Id)).ToList();
    public List<LearnerSummary> AvailableForSelectionLearners => Learners.Where(l => !SelectedLearnersIds.Contains(l.Id)).ToList();
    public string PageTitle => $"Select learners from the ILR";

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

    public DateTime? LastIlrSubmittedOn { get; set; }

    public string LastIlrSubmittedOnDesc
    {
        get
        {
            if (LastIlrSubmittedOn.HasValue == false)
            {
                return "";
            }

            var britishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var britishDateTime = TimeZoneInfo.ConvertTimeFromUtc(LastIlrSubmittedOn.Value, britishTimeZone);

            var timeString = britishDateTime.ToString("h:mmtt");
            // Ensure consistent AM/PM format across platforms
            timeString = timeString.Replace("am", "AM").Replace("pm", "PM");
            return $"Last updated {timeString} on {britishDateTime:dddd d MMMM}";
        }
    }
    public bool ShowPageLinks => FilterModel.TotalNumberOfLearnersFound > Constants.LearnerRecordSearch.NumberOfLearnersPerSearchPage;

    public MultipleLearnerRecordsFilterModel FilterModel { get; set; }
    public int FutureMonths { get; set; }
    public ApprenticeshipEmployerType LevyStatus { get; set; }
    public List<ValidationFailure> ValidationErrors { get; set; } = new List<ValidationFailure>();
    public int MaxSelectableLearners { get; set; }
}
using System.Collections.Generic;
using System.Security.AccessControl;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortViewModel
    {
        public long ProviderId { get; set; }

        public string SortedByHeaderClassName { get; set; }

        public const string HeaderClassName = "das-table__sort";

        public ChooseCohortFilterModel FilterModel { get; set; } 

        public void SortedByHeader()
        {
            SortedByHeaderClassName += HeaderClassName;
            if (ReverseSort)
            {
                SortedByHeaderClassName += " das-table__sort--desc";
            }
            else
            {
                SortedByHeaderClassName += " das-table__sort--asc";
            }
        }

        public IEnumerable<ChooseCohortSummaryViewModel> Cohorts { get; set; }
    }
}
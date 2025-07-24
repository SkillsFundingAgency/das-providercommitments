namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortViewModel
    {
        public long ProviderId { get; set; }

        public string SortedByHeaderClassName
        {
            get
            {
                string sortedByHeaderClassName = HeaderClassName;

                if (FilterModel.ReverseSort)
                {
                    sortedByHeaderClassName += " das-table__sort--desc";
                }
                else
                {
                    sortedByHeaderClassName += " das-table__sort--asc";
                }

                return sortedByHeaderClassName;
            }
        }

        public const string HeaderClassName = "das-table__sort";

        public ChooseCohortFilterModel FilterModel { get; set; } = new ChooseCohortFilterModel();

        public IEnumerable<ChooseCohortSummaryViewModel> Cohorts { get; set; }
    }
}
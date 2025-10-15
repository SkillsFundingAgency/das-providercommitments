using SFA.DAS.CommitmentsV2.Shared.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CohortDraftApprenticeshipViewModel
    {
        public CohortDraftApprenticeshipViewModel()
        {
            OverlappingTrainingDateRequest = new OverlappingTrainingDateRequestViewModel();
        }
        public long Id { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Cost { get; set; }
        public int? TrainingPrice { get; set; }
        public int? EndPointAssessmentPrice { get; set; }
        public int? FundingBandCap { get; set; }
        public string ULN { get; set; }
        public bool HasOverlappingUln { get; set; }
        public bool HasOverlappingEmail { get; set; }
        public OverlappingTrainingDateRequestViewModel OverlappingTrainingDateRequest { get; set; }
        public bool IsComplete { get; set; }
        public int? EmploymentPrice { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public bool HasLearnerDataChanges { get; set; }
        public DateTime? LastLearnerDataSync { get; set; }

        public bool ExceedsFundingBandCap
        {
            get
            {
                if (Cost.HasValue && FundingBandCap.HasValue)
                {
                    return Cost.Value > FundingBandCap.Value;
                }

                return false;
            }
        }

        public string DisplayName => $"{FirstName} {LastName}";

        public string DisplayDateOfBirth => DateOfBirth.HasValue ? DateOfBirth.Value.ToGdsFormat() : "-";

        public string DisplayTrainingDates
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    return $"{StartDate.Value.ToGdsFormatWithoutDay()} to {EndDate.Value.ToGdsFormatWithoutDay()}";
                }

                if (ActualStartDate.HasValue && EndDate.HasValue)
                {
                    return $"{ActualStartDate.Value.ToGdsFormat()} to {EndDate.Value.ToGdsFormat()}";
                }

                return "-";
            }
        }

        public string DisplayEmploymentDates
        {
            get
            {
                if (StartDate.HasValue && EmploymentEndDate.HasValue)
                {
                    return $"{StartDate.Value.ToGdsFormatWithoutDay()} to {EmploymentEndDate.Value.ToGdsFormatWithoutDay()}";
                }

                if (ActualStartDate.HasValue && EmploymentEndDate.HasValue)
                {
                    return $"{ActualStartDate.Value.ToGdsFormatWithoutDay()} to {EmploymentEndDate.Value.ToGdsFormatWithoutDay()}";
                }

                return "-";
            }
        }

        public string DisplayEndPointAssessmentPrice => EndPointAssessmentPrice?.ToGdsCostFormat();
        public string DisplayTrainingPrice => TrainingPrice?.ToGdsCostFormat();
        public string DisplayCost => Cost?.ToGdsCostFormat() ?? "-";

        public string DisplayEmploymentPrice => EmploymentPrice?.ToGdsCostFormat() ?? "-";

        public DateTime? OriginalStartDate { get; set; }
        public bool IsEditable { get; set; }

        public class OverlappingTrainingDateRequestViewModel
        {
            public DateTime? CreatedOn { get; set; }
        }
    }
}

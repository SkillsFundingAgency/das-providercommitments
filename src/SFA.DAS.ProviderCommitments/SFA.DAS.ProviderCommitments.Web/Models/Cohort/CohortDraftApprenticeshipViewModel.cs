using SFA.DAS.CommitmentsV2.Shared.Extensions;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CohortDraftApprenticeshipViewModel
    {
        public long Id { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Cost { get; set; }
        public int? FundingBandCap { get; set; }
        public string ULN { get; set; }
        public bool HasOverlappingUln { get; set; }
        public bool HasOverlappingEmail { get; set; }
        public bool IsComplete { get; set; }
        public int? EmploymentPrice { get; set; }
        public DateTime? EmploymentEndDate { get; set; }

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
                    return $"{ActualStartDate.Value.ToGdsFormatWithoutDay()} to {EndDate.Value.ToGdsFormatWithoutDay()}";
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

        public string DisplayCost => Cost?.ToGdsCostFormat() ?? "-";
            
        public string DisplayEmploymentPrice => EmploymentPrice?.ToGdsCostFormat() ?? "-";


        public DateTime? OriginalStartDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipViewModel
    {
        public DraftApprenticeshipViewModel(DateTime? dateOfBirth, DateTime? startDate, DateTime? actualStartDate, DateTime? endDate, DateTime? employmentEndDate = null) : base()
        {
            DateOfBirth = dateOfBirth == null ? new DateModel() : new DateModel(dateOfBirth.Value);
            StartDate = startDate == null ? new MonthYearModel("") : new MonthYearModel($"{startDate.Value.Month}{startDate.Value.Year}");
            ActualStartDate = actualStartDate == null ? new DateModel() : new DateModel(actualStartDate.Value);
            EndDate = endDate == null ? new DateModel() : new DateModel(endDate.Value);
            EmploymentEndDate = employmentEndDate == null ? new MonthYearModel("") : new MonthYearModel($"{employmentEndDate.Value.Month}{employmentEndDate.Value.Year}");
        }

        public DraftApprenticeshipViewModel()
        {
            DateOfBirth = new DateModel();
            StartDate = new MonthYearModel("");
            ActualStartDate = new DateModel();
            EndDate = new MonthYearModel("");
            EmploymentEndDate = new MonthYearModel("");
        }

        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public long? CohortId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }

        public Guid? ReservationId { get; set; }

        [Display(Name = "Employer")]
        [MaxLength(100)]
        public string Employer { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateModel DateOfBirth { get; }

        [Display(Name = "Day")]
        [SuppressArgumentException(nameof(DateOfBirth), "The Date of birth is not valid")]
        public int? BirthDay { get => DateOfBirth.Day; set => DateOfBirth.Day = value; }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(DateOfBirth), "The Date of birth is not valid")]
        public int? BirthMonth { get => DateOfBirth.Month; set => DateOfBirth.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(DateOfBirth), "The Date of birth is not valid")]
        public int? BirthYear { get => DateOfBirth.Year; set => DateOfBirth.Year = value; }

        [Display(Name = "Unique Learner Number (ULN)")]
        public string Uln { get; set; }

        public string CourseCode { get; set; }
        public string CourseName { get; set; }

        [Display(Name = "Planned apprenticeship training start date")]
        public MonthYearModel StartDate { get; set; }

        [Display(Name = "Day")]
        [SuppressArgumentException(nameof(ActualStartDate), "The start date is not valid")]
        public int? ActualStartDay { get => ActualStartDate.Day; set => ActualStartDate.Day = value; }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(ActualStartDate), "The start date is not valid")]
        public int? ActualStartMonth { get => ActualStartDate.Month; set => ActualStartDate.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(ActualStartDate), "The start date is not valid")]
        public int? ActualStartYear { get => ActualStartDate.Year; set => ActualStartDate.Year = value; }

        [Display(Name = "Actual apprenticeship training start date")]
        [DataType(DataType.Date)]
        public DateModel ActualStartDate { get; set; }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        [Display(Name = "Planned apprenticeship training finish date")]
        public DateModel EndDate { get; private set; }

        [Display(Name = "Day")]
        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndDay
        {
            get => EndDate.Day;
            set
            {
                if (EndDate.GetType() != typeof(MonthYearModel))
                    EndDate.Day = value;
            }
        }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }

        [Display(Name = "Planned end date for this employment")]
        public MonthYearModel EmploymentEndDate { get; }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(EmploymentEndDate), "The employment end date is not valid")]
        public int? EmploymentEndMonth { get => EmploymentEndDate.Month; set => EmploymentEndDate.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(EndDate), "The employment end date is not valid")]
        public int? EmploymentEndYear { get => EmploymentEndDate.Year; set => EmploymentEndDate.Year = value; }

        [Display(Name = "Total agreed apprenticeship price (excluding VAT)")]
        [IntegerLengthCheck(nameof(Cost), "Total agreed apprenticeship price (excluding VAT)", 7)]
        public int? Cost { get; set; }
     
        [Display(Name = "Training Price")]
        [SuppressArgumentException(nameof(TrainingPrice), "Training price must be a whole number")]
        public int? TrainingPrice { get; set; }

        [Display(Name = "End-point assessment price")]
        [SuppressArgumentException(nameof(EndPointAssessmentPrice), "End-point assessment price must be a whole number")]
        public int? EndPointAssessmentPrice { get; set; }

        [Display(Name = "Agreed price for this employment (excluding VAT)")]
        [SuppressArgumentException(nameof(EmploymentPrice), "Agreed employment price must be 7 numbers or fewer")]
        public int? EmploymentPrice { get; set; }

        [Display(Name = "Reference (optional)")]
        public string Reference { get; set; }
        public TrainingProgramme[] Courses { get; set; }
        public bool IsContinuation { get; set; }
        public bool HasStandardOptions { get; set; }
        public string TrainingCourseOption { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public bool? RecognisePriorLearning { get; set; }
        public int? DurationReducedBy { get; set; }
        public int? PriceReducedBy { get; set; }
        public bool RecognisingPriorLearningStillNeedsToBeConsidered { get; set; }
        public bool RecognisingPriorLearningExtendedStillNeedsToBeConsidered { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }
        public bool HasUnavailableFlexiJobAgencyDeliveryModel { get; set; }
        public bool HasChangedDeliveryModel { get; set; }
        public bool? EmailAddressConfirmed { get; set; }
        public bool? EmployerHasEditedCost { get; set; }
        public long? LearnerDataId { get; set; }
        public bool HasLearnerDataChanges { get; set; }
        public DateTime? LastLearnerDataSync { get; set; }
        public string Name { get; set; }
        public ViewEditBanners Banner { get; set; }
    }
}
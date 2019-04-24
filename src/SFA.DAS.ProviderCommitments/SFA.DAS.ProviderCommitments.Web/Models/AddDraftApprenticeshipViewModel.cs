using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel
    {
        public AddDraftApprenticeshipViewModel()
        {
            DateOfBirth = new DateModel(); 
            EndDate = new MonthYearModel("");
            StartDate = new MonthYearModel("");
        }

        public Guid ReservationId { get; set; }
        public int ProviderId { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }

        [Display(Name = "Employer")]
        [MaxLength(100)]
        public string Employer { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateModel DateOfBirth { get; }

        [Display(Name = "Day")]
        public int? BirthDay { get => DateOfBirth.Day ; set => DateOfBirth.Day = value; }

        [Display(Name = "Month")]
        public int? BirthMonth { get => DateOfBirth.Month; set => DateOfBirth.Month = value; }

        [Display(Name = "Year")]
        public int? BirthYear { get => DateOfBirth.Year; set => DateOfBirth.Year = value; }

        [Display(Name = "Unique Learner Number (ULN)")]
        public string Uln { get; set; }

        public string CourseCode { get; set; }

        [Display(Name = "Planned training start date")]
        public MonthYearModel StartDate { get; set; }

        [Display(Name = "Month")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }

        [Display(Name = "Year")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        [Display(Name = "Projected finish date")]
        public MonthYearModel EndDate { get; }

        [Display(Name = "Month")]
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }

        [Display(Name = "Year")]
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }

        [Display(Name = "Total agreed apprenticeship price (excluding VAT)")]
        public int? Cost { get; set; }

        [Display(Name = "Reference (optional)")]
        public string Reference { get; set; }

        public ICourse[] Courses { get; set; }
    }
}
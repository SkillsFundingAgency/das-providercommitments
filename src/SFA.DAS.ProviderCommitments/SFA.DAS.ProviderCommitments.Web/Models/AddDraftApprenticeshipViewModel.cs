using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel
    {
        public AddDraftApprenticeshipViewModel()
        {
            BirthDate = new DateModel(); 
            FinishDate = new MonthYearModel("");
            StartDate = new MonthYearModel("");
        }

        public Guid ReservationId { get; set; }
        public int ProviderId { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }

        [Display(Name = "Employer")]
        [MaxLength(100)]
        public string Employer { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateModel BirthDate { get; }

        [Display(Name = "Day")]
        public int? BirthDay { get => BirthDate.Day ; set => BirthDate.Day = value; }

        [Display(Name = "Month")]
        public int? BirthMonth { get => BirthDate.Month; set => BirthDate.Month = value; }

        [Display(Name = "Year")]
        public int? BirthYear { get => BirthDate.Year; set => BirthDate.Year = value; }

        [Display(Name = "Unique Learner Number (ULN)")]
        public string UniqueLearnerNumber { get; set; }

        public string CourseCode { get; set; }

        [Display(Name = "Apprenticeship course")]
        public string CourseName { get; set; }

        [Display(Name = "Planned training start date")]
        public MonthYearModel StartDate { get; set; }

        [Display(Name = "Month")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }

        [Display(Name = "Year")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        [Display(Name = "Projected finish date")]
        public MonthYearModel FinishDate { get; }

        [Display(Name = "Month")]
        public int? FinishMonth { get => FinishDate.Month; set => FinishDate.Month = value; }

        [Display(Name = "Year")]
        public int? FinishYear { get => FinishDate.Year; set => FinishDate.Year = value; }

        [Display(Name = "Total agreed apprenticeship price (excluding VAT)")]
        public int? Cost { get; set; }

        [Display(Name = "Reference (optional)")]
        public string Reference { get; set; }

        public ICourse[] Courses { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class EditApprenticeshipViewModel
    {
        public EditApprenticeshipViewModel(string startMonthYear)
        {
            BirthDate = new DateModel();    
            StartDate = new MonthYearModel(startMonthYear);
            FinishDate = new DateModel();
            Courses = new List<SelectListItem>();
        }

        public long ReservationId { get; set; }

        [Required]
        [Display(Name = "Employer")]
        [MaxLength(100)]
        public string Employer { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateModel BirthDate { get; }

        [Required]
        [Display(Name = "Day")]
        public int BirthDay { get => BirthDate.Day ; set => BirthDate.Day = value; }

        [Required]
        [Display(Name = "Month")]
        public int BirthMonth { get => BirthDate.Month; set => BirthDate.Month = value; }

        [Required]
        [Display(Name = "Year")]
        public int BirthYear { get => BirthDate.Year; set => BirthDate.Year = value; }

        [Required]
        [Display(Name = "Unique Learner Number (ULN)")]
        public string UniqueLearnerNumber { get; set; }

        public string CourseCode { get; set; }

        [Required]
        [Display(Name = "Apprenticeship course")]
        public string CourseName { get; set; }

        [Display(Name = "Planned training start date")]
        public MonthYearModel StartDate { get; }

        [Required]
        [Display(Name = "Month")]
        public int StartMonth { get => StartDate.Month; set => StartDate.Month = value; }

        [Required]
        [Display(Name = "Year")]
        public int StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        [Display(Name = "Projected finish date")]
        public DateModel FinishDate { get; }

        [Required]
        [Display(Name = "Month")]
        public int FinishMonth { get => FinishDate.Month; set => FinishDate.Month = value; }

        [Required]
        [Display(Name = "Year")]
        public int FinishYear { get => FinishDate.Year; set => FinishDate.Year = value; }

        public List<SelectListItem> Courses { get; set; }
    }
}
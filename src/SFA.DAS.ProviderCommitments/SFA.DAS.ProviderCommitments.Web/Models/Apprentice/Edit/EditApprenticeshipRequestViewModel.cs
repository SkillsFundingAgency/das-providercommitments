﻿using System.ComponentModel.DataAnnotations;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class EditApprenticeshipRequestViewModel : IAuthorizationContextModel
    {
        public EditApprenticeshipRequestViewModel()
        {
            DateOfBirth = new DateModel();
            StartDate = new MonthYearModel("");
            EndDate = new MonthYearModel("");
            EmploymentEndDate = new MonthYearModel("");
        }

        public EditApprenticeshipRequestViewModel(DateTime? dateOfBirth, DateTime? startDate, DateTime? endDate, DateTime? employmentEndDate = null) : base()
        {
            DateOfBirth = dateOfBirth == null ? new DateModel() : new DateModel(dateOfBirth.Value);
            StartDate = startDate == null ? new MonthYearModel("") : new MonthYearModel($"{startDate.Value.Month}{startDate.Value.Year}");
            EndDate = endDate == null ? new MonthYearModel("") : new MonthYearModel($"{endDate.Value.Month}{endDate.Value.Year}");
            EmploymentEndDate = employmentEndDate == null ? new MonthYearModel("") : new MonthYearModel($"{employmentEndDate.Value.Month}{employmentEndDate.Value.Year}");
        }
        public string EmployerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string ULN { get; set; }
        public string TrainingName { get; set; }
        public string Version { get; set; }
        public string Option { get; set; }

        [SuppressArgumentException(nameof(Cost), "The apprenticeship price is not valid")]
        public decimal? Cost { get; set; }

        public string ProviderReference { get; set; }

        public bool IsContinuation { get; set; }

        [DataType(DataType.Date)]
        public DateModel DateOfBirth { get; set; }

        [SuppressArgumentException(nameof(DateOfBirth), "The date of birth is not valid")]
        public int? BirthDay { get => DateOfBirth.Day; set => DateOfBirth.Day = value; }

        [SuppressArgumentException(nameof(DateOfBirth), "The date of birth is not valid")]
        public int? BirthMonth { get => DateOfBirth.Month; set => DateOfBirth.Month = value; }

        [SuppressArgumentException(nameof(DateOfBirth), "The date of birth is not valid")]
        public int? BirthYear { get => DateOfBirth.Year; set => DateOfBirth.Year = value; }

        public string CourseCode { get; set; }

        public IEnumerable<TrainingProgramme> Courses { get; set; }

        public MonthYearModel StartDate { get; set; }

        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartMonth { get => StartDate.Month; set => StartDate.Month = value; }

        [SuppressArgumentException(nameof(StartDate), "The start date is not valid")]
        public int? StartYear { get => StartDate.Year; set => StartDate.Year = value; }

        public MonthYearModel EndDate { get; set; }

        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndMonth { get => EndDate.Month; set => EndDate.Month = value; }

        [SuppressArgumentException(nameof(EndDate), "The end date is not valid")]
        public int? EndYear { get => EndDate.Year; set => EndDate.Year = value; }

        public bool IsLockedForUpdate { get; set; }

        public bool IsUpdateLockedForStartDateAndCourse { get; set; }
        public bool IsEndDateLockedForUpdate { get; internal set; }
        public long ProviderId { get; set; }
        public bool EmailAddressConfirmedByApprentice { get; set; }
        public bool EmailShouldBePresent { get; set; }
        public bool HasOptions { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string CourseName { get; set; }

        public int? EmploymentPrice { get; set; }

        [Display(Name = "Planned end date for this employment")]
        public MonthYearModel EmploymentEndDate { get; }

        [Display(Name = "Month")]
        [SuppressArgumentException(nameof(EmploymentEndDate), "The employment end date is not valid")]
        public int? EmploymentEndMonth { get => EmploymentEndDate.Month; set => EmploymentEndDate.Month = value; }

        [Display(Name = "Year")]
        [SuppressArgumentException(nameof(EmploymentEndDate), "The employment end date is not valid")]
        public int? EmploymentEndYear { get => EmploymentEndDate.Year; set => EmploymentEndDate.Year = value; }

        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }
    }
}

﻿using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class BaseEdit : IAuthorizationContextModel
    {
        [JsonIgnore]
        public string Name => FirstName + " " + LastName;

        [FromRoute]
        public long ProviderId { get; set; }
        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
        [JsonIgnore]
        public long ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ULN { get; set; }
        public string CourseName { get; set; }
        public string Version { get; set; }
        public string Option { get; set; }
        public decimal? Cost { get; set; }
        public string ProviderReference { get; set; }

        [JsonIgnore]
        public DateTime? DateOfBirth
        {
            get
            {
                if (BirthDay.HasValue && BirthYear.HasValue && BirthMonth.HasValue)
                {
                    return new DateTime(BirthYear.Value, BirthMonth.Value, BirthDay.Value);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    BirthDay = value.Value.Day;
                    BirthMonth = value.Value.Month;
                    BirthYear = value.Value.Year;
                }
            }
        }

        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }
        public string CourseCode { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }

        [JsonIgnore]
        public DateTime? StartDate
        {
            get
            {
                if (StartMonth.HasValue && StartYear.HasValue)
                {
                    return new DateTime(StartYear.Value, StartMonth.Value, 1);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    StartMonth = value.Value.Month;
                    StartYear = value.Value.Year;
                }
            }
        }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }

        [JsonIgnore]
        public DateTime? EndDate
        {
            get
            {
                if (EndMonth.HasValue && EndYear.HasValue)
                {
                    return new DateTime(EndYear.Value, EndMonth.Value, 1);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    EndMonth = value.Value.Month;
                    EndYear = value.Value.Year;
                }
            }
        }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }

        [JsonIgnore]
        public DateTime? EmploymentEndDate
        {
            get
            {
                if (EmploymentEndMonth.HasValue && EmploymentEndYear.HasValue)
                {
                    return new DateTime(EmploymentEndYear.Value, EmploymentEndMonth.Value, 1);
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    EmploymentEndMonth = value.Value.Month;
                    EmploymentEndYear = value.Value.Year;
                }
            }
        }
        public int? EmploymentEndMonth { get; set; }
        public int? EmploymentEndYear { get; set; }
        public int? EmploymentPrice { get; set; }
    }
}

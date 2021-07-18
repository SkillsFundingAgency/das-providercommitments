using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class UpdateDateLockViewModel : IAuthorizationContextModel
    {      
        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
        [JsonIgnore]
        public long ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ULN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CourseName { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }        

        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        //public DateTime? IlrEffectiveFromDate { get; internal set; }
        //public DateTime? IlrEffectiveToDate { get; internal set; }
        public IEnumerable<PriceHistoryViewModel> PriceDataLocks { get; set; }

        public IEnumerable<CourseDataLockViewModel> CourseDataLocks { get; set; }

        public int TotalChanges => (PriceDataLocks?.Count() ?? 0) + (CourseDataLocks?.Count() ?? 0);
    }


    public class UpdateDateLockSummaryViewModel
    {
        public IList<DataLockViewModel> DataLockWithOnlyPriceMismatch { get; set; }

        public IList<DataLockViewModel> DataLockWithCourseMismatch { get; set; }
    }

    public class DataLockViewModel
    {
        //public long DataLockEventId { get; set; }

        public DateTime DataLockEventDatetime { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public long ApprenticeshipId { get; set; }

        public string IlrTrainingCourseCode { get; set; }

        public string IlrTrainingCourseName { get; set; }

        //public TrainingType IlrTrainingType { get; set; }

        public DateTime? IlrActualStartDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }
        public DateTime? IlrEffectiveToDate { get; set; }

        public decimal? IlrTotalCost { get; set; }

        public TriageStatusViewModel TriageStatusViewModel { get; set; }

        public DataLockErrorCode DataLockErrorCode { get; set; }
    }

    public enum TrainingType
    {
        Standard = 0,
        Framework = 1
    }

    public enum TriageStatusViewModel
    {
        Unknown = 0,
        ChangeApprenticeship = 1,
        RestartApprenticeship = 2,
        FixInIlr = 3
    }
    
}

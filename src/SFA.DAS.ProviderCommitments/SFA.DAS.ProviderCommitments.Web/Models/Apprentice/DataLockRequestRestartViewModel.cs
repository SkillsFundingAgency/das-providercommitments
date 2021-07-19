using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.ModelBinding;
using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DataLockRequestRestartViewModel : IAuthorizationContextModel
    {
        [FromRoute]
        public string AccountHashedId { get; set; }
        [JsonIgnore]
        public long AccountId { get; set; }
        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
        [JsonIgnore]
        public long ApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ULN { get; set; }
        public string CourseName { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }        
        public string NewCourseCode { get; internal set; }
        public string NewCourseName { get; internal set; }        
        public DateTime? IlrEffectiveFromDate { get; internal set; }
        public DateTime? IlrEffectiveToDate { get; internal set; }
        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }        
    }

    public enum SubmitStatusViewModel
    {
        None = 1,
        Confirm = 2,
        UpdateDataInIlr = 3,
    }
}

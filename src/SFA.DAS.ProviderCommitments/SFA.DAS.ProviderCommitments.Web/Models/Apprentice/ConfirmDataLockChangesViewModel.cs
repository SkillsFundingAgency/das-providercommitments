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
    public class ConfirmDataLockChangesViewModel : IAuthorizationContextModel
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
        public string EmployerName { get; set; } //TODO : need to add
        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        //public DateTime? IlrEffectiveFromDate { get; internal set; }
        //public DateTime? IlrEffectiveToDate { get; internal set; }
        public IEnumerable<PriceHistoryViewModel> PriceDataLocks { get; set; }

        public IEnumerable<CourseDataLockViewModel> CourseDataLocks { get; set; }

        public int TotalChanges => (PriceDataLocks?.Count() ?? 0) + (CourseDataLocks?.Count() ?? 0);
    }
}

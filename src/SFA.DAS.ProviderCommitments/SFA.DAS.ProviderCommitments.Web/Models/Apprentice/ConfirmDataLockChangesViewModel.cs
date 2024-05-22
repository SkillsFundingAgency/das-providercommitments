using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

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
        public string EmployerName { get; set; }
        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        public IEnumerable<PriceDataLockViewModel> PriceDataLocks { get; set; }

        public IEnumerable<CourseDataLockViewModel> CourseDataLocks { get; set; }

        public int TotalChanges => (PriceDataLocks?.Count() ?? 0) + (CourseDataLocks?.Count() ?? 0);
    }
}

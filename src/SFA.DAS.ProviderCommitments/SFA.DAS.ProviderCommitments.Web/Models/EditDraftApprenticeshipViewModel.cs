using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class EditDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IDraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public EditDraftApprenticeshipViewModel(DateTime? dateOfBirth, DateTime? startDate, DateTime? actualStartDate, DateTime? endDate, DateTime? employmentEndDate = null) : base(dateOfBirth, startDate, actualStartDate, endDate, employmentEndDate)
        {
        }

        public EditDraftApprenticeshipViewModel()
        {
        }

        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
    }
}
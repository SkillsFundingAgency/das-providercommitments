using System;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class EditDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IDraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public EditDraftApprenticeshipViewModel(DateTime? dateOfBirth, DateTime? startDate, DateTime? endDate, DateTime? employmentEndDate = null) : base(dateOfBirth, startDate, endDate, employmentEndDate)
        {
        }

        public EditDraftApprenticeshipViewModel()
        {
        }

        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
    }
}
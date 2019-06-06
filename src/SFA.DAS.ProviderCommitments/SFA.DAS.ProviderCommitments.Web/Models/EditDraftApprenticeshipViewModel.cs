using System;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class EditDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public EditDraftApprenticeshipViewModel(DateTime? dateOfBirth, DateTime? startDate, DateTime? endDate) : base(dateOfBirth, startDate, endDate)
        {
        }

        public EditDraftApprenticeshipViewModel()
        {
        }

        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
    }
}
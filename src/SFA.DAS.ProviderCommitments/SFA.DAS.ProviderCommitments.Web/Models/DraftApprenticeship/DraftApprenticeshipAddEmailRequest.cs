using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class DraftApprenticeshipAddEmailRequest: DraftApprenticeshipRequest
    {
      
        public string Email { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}

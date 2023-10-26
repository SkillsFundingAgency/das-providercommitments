using System.Collections.Generic;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public sealed class AcknowledgementRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public long ProviderId { get; set; }
        [FromRoute]
        public string CohortReference { get; set; }
        public long CohortId { get; set; }
        public SaveStatus SaveStatus { get; set; }
    }
}
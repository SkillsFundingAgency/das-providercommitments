using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ChangeOptionRequest : IAuthorizationContextModel
    {
        [FromRoute]
        public long ProviderId { get; set; }

        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}

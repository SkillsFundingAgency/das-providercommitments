using SFA.DAS.Authorization.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class ChangeEmployerRequest : IAuthorizationContextModel
    {
        public long ApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long ProviderId { get; set; }
    }
}

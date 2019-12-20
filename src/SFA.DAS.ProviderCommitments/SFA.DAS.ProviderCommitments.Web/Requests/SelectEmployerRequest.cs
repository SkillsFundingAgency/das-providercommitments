using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class SelectEmployerRequest
    {
        public long ProviderId { get; set; }
    }
}

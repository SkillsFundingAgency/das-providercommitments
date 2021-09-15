using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortByProviderRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
    }
}

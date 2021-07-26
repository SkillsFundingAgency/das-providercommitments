using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit
{
    public class ConfirmEditApprenticeshipViewModel : BaseEdit
    {
        public bool? ConfirmChanges { get; set; }

        public BaseEdit OriginalApprenticeship { get; set; }
    }
}

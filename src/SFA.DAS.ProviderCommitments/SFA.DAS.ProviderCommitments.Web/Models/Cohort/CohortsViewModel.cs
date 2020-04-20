using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CohortsViewModel
    {
        public long ProviderId { get; set; }
        public bool ShowDrafts { get; set; }
        public bool HasPermissionsToCreateCohorts { get; set; }
        public CohortCardLinkViewModel CohortsInDraft { get; set; }
        public CohortCardLinkViewModel CohortsInReview { get; set; }
        public CohortCardLinkViewModel CohortsWithEmployer { get; set; }
        public CohortCardLinkViewModel CohortsWithTransferSender { get; set; }
    }
}

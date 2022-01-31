using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadAmendedFileViewModel
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public bool? Confirm { get; set; }
    }
}

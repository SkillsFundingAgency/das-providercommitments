using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class BlobStorageSettings
    {
        public string ConnectionString { get; set; }
        public string BulkuploadContainer { get; set; }
    }
}

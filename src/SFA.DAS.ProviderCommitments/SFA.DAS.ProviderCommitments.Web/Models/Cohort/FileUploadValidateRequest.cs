using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadValidateRequest
    {
        public long ProviderId { get; set; }
        public IFormFile  Attachement { get; set; }
    }
}

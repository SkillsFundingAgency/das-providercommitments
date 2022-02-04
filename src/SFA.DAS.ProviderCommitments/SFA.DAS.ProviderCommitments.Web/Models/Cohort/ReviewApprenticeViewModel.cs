using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewApprenticeViewModel
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public string EmployerName { get; set; }
        public string Cohort { get; set; }
        public string MsgFromEmployer { get; set; }
        public int TotalApprentices { get; set; }
        public decimal TotalCost { get; set; }

        public List<ReviewApprenticeDetails> CohortDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewApprenticeViewModel
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public string EmployerName { get; set; }
        public string CohortRef { get; set; }        
        public int TotalApprentices { get; set; }        
        public decimal TotalCost { get; set; }
        public string CohortRefText => CohortRef ?? "This will be created when you save or send to employers";
        public string MessageFromEmployer { get; set; }
        public string MessageFromEmployerText => MessageFromEmployer ?? "No message added.";

        public List<ReviewApprenticeDetails> CohortDetails { get; set; }

        public string FundingBandText
        {
            get
            {
                int count = (CohortDetails.Where(apprentice => apprentice.ExceedsFundingBandCap)).Count();
                if (count == 0) return string.Empty;

                var text = count > 1 ? $"{count} apprenticeships above funding band maximum" : "1 apprenticeship above funding band maximum";
                return text;
            }
        }
    }
}

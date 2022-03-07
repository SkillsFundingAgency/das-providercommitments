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

        public string CsvTotalApprenticesText { get; set; }
        public string DbTotalApprenticesText { get; set; }

        //cohortDetailfromCsv.NumberOfApprentices = cohortGroup.Count();
        //       cohortDetailfromCsv.TotalCost = cohortGroup.Sum(x => int.Parse(x.TotalPrice));

        public List<ReviewApprenticeDetails> ExistingCohortDetails { get; set; } //ExistingCohortDetails

        public List<FileUploadReviewApprenticeDetails> FileUploadCohortDetails { get; set; } //FileUploadCohortDetails

        public string FileUploadFundingBandText
        {
            get
            {
                int count = (FileUploadCohortDetails.Where(apprentice => apprentice.ExceedsFundingBandCap)).Count();
                if (count == 0) return string.Empty;

                var text = count > 1 ? $"{count} apprenticeships above funding band maximum" : "1 apprenticeship above funding band maximum";
                return text;
            }
        }

        public string ExistingFundingBandText
        {
            get
            {
                int count = (ExistingCohortDetails.Where(apprentice => apprentice.ExceedsFundingBandCap)).Count();
                if (count == 0) return string.Empty;

                var text = count > 1 ? $"{count} apprenticeships above funding band maximum" : "1 apprenticeship above funding band maximum";
                return text;
            }
        }
    }
}

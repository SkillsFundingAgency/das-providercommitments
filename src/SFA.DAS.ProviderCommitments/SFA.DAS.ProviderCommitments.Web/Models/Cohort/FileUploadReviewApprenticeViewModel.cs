using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewApprenticeViewModel
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public string EmployerName { get; set; }
        public string CohortRef { get; set; }
        public string CohortRefText => CohortRef ?? "This will be created when you save or send to employers";
        public string CohortRef { get; set; }        
        public int TotalApprentices { get; set; }        
        public decimal TotalCost { get; set; }
        public string CohortRefText => string.IsNullOrWhiteSpace(CohortRef) ? "This will be created when you save or send to employers" : CohortRef;
        public string MessageFromEmployer { get; set; }
        public string MessageFromEmployerText => MessageFromEmployer ?? "No message added.";
        public List<ReviewApprenticeDetailsForFileUploadCohort> FileUploadCohortDetails { get; set; }
        public List<ReviewApprenticeDetailsForExistingCohort> ExistingCohortDetails { get; set; }
        public int? TotalApprentices => FileUploadCohortDetails?.Count() + ExistingCohortDetails?.Count();        
        public decimal? TotalCost => FileUploadCohortDetails?.Sum(x => x.Price ?? 0) + ExistingCohortDetails?.Sum(x => x.Price ?? 0);
        public string FundingBandLink {
            get 
            {
                return "https://www.gov.uk/government/publications/apprenticeship-funding-bands";
            }
        }

        public string FileUploadTotalApprenticesText
        public List<FileUploadReviewApprenticeDetails> CohortDetails { get; set; }

        public string FundingBandText
        {
            get
            {
                int? count = FileUploadCohortDetails?.Count();

                if (count == 0) return string.Empty;

                var text = count > 1 ? $"{count} apprentices to be added from CSV file" : "1 apprentice to be added from CSV file";
                return text;
            }
        }

        public string ExistingCohortTotalApprenticesText {
            get
            {
                int? count = ExistingCohortDetails?.Count();

                if (count == 0) return string.Empty;

                var text = count > 1 ? $"{count} apprentices previously added to this cohort" : "1 apprentice previously added to this cohort";
                return text;
            }
        }

        public string FundingBandTextForFileUploadCohorts
        {
            get
            {
                int count = (FileUploadCohortDetails?.Where(apprentice => apprentice.ExceedsFundingBandCap)).Count();
                return FundingBandText(count);
            }
        }       

        public string FundingBandTextForExistingCohorts
        {
            get
            {
                int count = (ExistingCohortDetails?.Where(apprentice => apprentice.ExceedsFundingBandCapForExistingCohort)).Count();
                return FundingBandText(count);
            }
        }

        private static string FundingBandText(int count)
        {
            if (count == 0) return string.Empty;

            var text = count > 1 ? $"{count} apprenticeships above funding band maximum" : "1 apprenticeship above funding band maximum";
            return text;
        }        
    }
}

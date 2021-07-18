using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class CourseDataLockViewModel
    {
        public string TrainingName { get; set; }

        public DateTime ApprenticeshipStartDate { get; set; }

        public string IlrTrainingName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }
    }
}

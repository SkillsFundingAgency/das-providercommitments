using SFA.DAS.CommitmentsV2.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class UpdateDateLockSummaryViewModel
    {
        public IList<DataLockViewModel> DataLockWithPriceMismatch { get; set; }

        public IList<DataLockViewModel> DataLockWithCourseMismatch { get; set; }
    }

    public class DataLockViewModel
    {
        public DateTime DataLockEventDatetime { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long ApprenticeshipId { get; set; }
        public string IlrTrainingCourseCode { get; set; }
        public string IlrTrainingCourseName { get; set; }
        public DateTime? IlrActualStartDate { get; set; }
        public DateTime? IlrEffectiveFromDate { get; set; }
        public DateTime? IlrPriceEffectiveToDate { get; set; }
        public decimal? IlrTotalCost { get; set; }
        public DataLockErrorCode DataLockErrorCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceHistoryViewModel
    {
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; } // CurrentCost

        public DateTime FromDate { get; set; } //CurrentStartDate

        public DateTime? ToDate { get; set; } // CurrentEndDate

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }

        public decimal? IlrTotalCost { get; set; }
    }
}

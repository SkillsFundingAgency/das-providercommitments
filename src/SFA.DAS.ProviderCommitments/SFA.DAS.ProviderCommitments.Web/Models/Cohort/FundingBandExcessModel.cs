﻿namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FundingBandExcessModel
    {
        private readonly int[] _fundingBandCapsExceeded;

        public FundingBandExcessModel(int numberOfApprenticesExceedingFundingBandCap, IEnumerable<int?> fundingBandCapsExceeded,
            string fundingBandCapExcessHeader = "", string fundingBandCapExcessLabel = "")
        {
            _fundingBandCapsExceeded = fundingBandCapsExceeded.Where(x => x.HasValue).Select(x => x.Value).ToArray();
            NumberOfApprenticesExceedingFundingBandCap = numberOfApprenticesExceedingFundingBandCap;
            FundingBandCapExcessHeader = fundingBandCapExcessHeader;
            FundingBandCapExcessLabel = fundingBandCapExcessLabel;
        }

        public int NumberOfApprenticesExceedingFundingBandCap { get; }

        public string DisplaySingleFundingBandCap => _fundingBandCapsExceeded.Length == 1 ? $" of £{_fundingBandCapsExceeded[0]:#,0}." : ".";
        public string FundingBandCapExcessHeader { get; set; }
        public string FundingBandCapExcessLabel { get; set; }
    }
}

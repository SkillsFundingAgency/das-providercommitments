using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangePriceRequest : ChangeStartDateRequest
    {
        public ChangePriceRequest()
        {
            NewStartDate = new MonthYearModel("");
        }
        public MonthYearModel NewStartDate { get; set; }
    }
}

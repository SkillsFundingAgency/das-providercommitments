using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public sealed class AcknowledgementViewModel
    {
        public string CohortReference { get;  set; }
        public string EmployerName { get;  set; }
        public string Message { get; set; }
        public string ProviderName { get;  set; }
        public long ProviderId { get;  set; }
        public long CohortId { get; set; }
        public string PageTitle { get; set; }
        public List<string> WhatHappensNext { get; set; }
        public bool CohortApproved { get; set; }
        public bool IsTransfer { get;  set; }
        public long? ChangeOfPartyRequestId { get;  set; }
        public Party WithParty { get; set; }
    }
}
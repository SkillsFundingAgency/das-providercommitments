﻿namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ConfirmEmployerViewModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountPublicHashedId { get; set; }
        public string EmployerAccountName { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityName { get; set; }
        public bool? Confirm { get; set; }
        public string BackLink { get; set; }
    }
}

﻿namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public enum SaveStatus
    {
        Save = 1,
        Approve = 2,
        ApproveAndSend = 3,
        AmendAndSend = 4
    }

    public static class SaveStatusExtension
    {
        public static bool IsSend(this SaveStatus status)
        {
            return status == SaveStatus.ApproveAndSend || status == SaveStatus.AmendAndSend;
        }
    }
}
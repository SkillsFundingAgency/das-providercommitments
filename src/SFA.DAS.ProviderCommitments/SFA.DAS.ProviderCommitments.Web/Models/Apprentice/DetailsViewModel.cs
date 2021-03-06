﻿using System;
using System.Collections.Generic;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DetailsViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string ApprenticeName { get; set; }
        public string Employer { get; set; }
        public string Reference { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? PauseDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string AgreementId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Uln { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderRef { get; set; }
        public decimal Cost { get; set; }
        public bool IsChangeOfEmployerEnabled { get; set; }
        public bool AllowEditApprentice { get; set; }
        public bool HasProviderPendingUpdate { get; set; }
        public bool HasEmployerPendingUpdate { get; set; }
        public DataLockSummaryStatus DataLockStatus { get; set; }
        public bool HasPendingChangeOfPartyRequest { get; set; }
        public Party? PendingChangeOfPartyRequestWithParty { get; set; }
        public bool HasApprovedChangeOfPartyRequest { get; set; }
        public bool HasPendingChangeOfProviderRequest { get; set; }
        public string EncodedNewApprenticeshipId { get; set; }
        public bool HasContinuation { get; set; }
        public string EncodedPreviousApprenticeshipId { get; set; }
        public List<EmployerHistory> EmployerHistory { get; set; }

        public bool SuppressDataLockStatusReviewLink => HasEmployerPendingUpdate || HasProviderPendingUpdate;
        public TriageOption AvailableTriageOption { get; set; }
        public ConfirmationStatus? ConfirmationStatus { get; set; }
        public bool ShowConfirmationStatus { get; set; }
        public string Email { get; set; }
        public enum DataLockSummaryStatus
        {
            None,
            AwaitingTriage,
            HasUnresolvedDataLocks
        }

        public enum TriageOption
        {
            Restart,
            Update,
            Both
        }
    }

    public class EmployerHistory
    {
        public string EmployerName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string HashedApprenticeshipId { get; set; }
        public bool ShowLink { get; set; }
    }
}

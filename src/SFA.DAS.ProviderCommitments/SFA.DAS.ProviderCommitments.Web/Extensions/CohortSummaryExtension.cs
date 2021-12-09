using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class CohortSummaryExtension
    {
        public static CohortStatus GetStatus(this CohortSummary cohort)
        {
            if (cohort.IsDraft && cohort.WithParty == Party.Provider)
                return CohortStatus.Draft;

            if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
                return CohortStatus.Review;

            if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
                return CohortStatus.WithEmployer;

            if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
                return CohortStatus.WithTransferSender;

            return CohortStatus.Unknown;
        }

        public static ApprenticeshipRequestsHeaderViewModel GetCohortCardLinkViewModel(
            this CohortSummary[] cohorts,
            IUrlHelper urlHelper, 
            long providerId, 
            CohortStatus selectedStatus, 
            bool hasRelationship,
            ProviderAgreementStatus providerAgreementStatus)
        {
            return new ApprenticeshipRequestsHeaderViewModel
            {
                ProviderId = providerId,
                ShowDrafts = hasRelationship,
                CohortsInDraft = new ApprenticeshipRequestsTabViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.Draft),
                    cohorts.Count(x => x.GetStatus() == CohortStatus.Draft) == 1 ? "Draft" : "Drafts",
                    urlHelper.Action("Draft", "Cohort", new {providerId}),
                    CohortStatus.Draft.ToString(),
                    selectedStatus == CohortStatus.Draft),
                CohortsInReview = new ApprenticeshipRequestsTabViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.Review),
                    "Ready to review",
                    urlHelper.Action("Review", "Cohort", new {providerId}),
                    CohortStatus.Review.ToString(),
                    selectedStatus == CohortStatus.Review),
                CohortsWithEmployer = new ApprenticeshipRequestsTabViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.WithEmployer),
                    "With employers",
                    urlHelper.Action("WithEmployer", "Cohort", new {providerId}),
                    CohortStatus.WithEmployer.ToString(),
                    selectedStatus == CohortStatus.WithEmployer),
                CohortsWithTransferSender = new ApprenticeshipRequestsTabViewModel(
                    cohorts.Count(x => x.GetStatus() == CohortStatus.WithTransferSender),
                    "With transfer sending employers",
                    urlHelper.Action("WithTransferSender", "Cohort", new {providerId}),
                    CohortStatus.WithTransferSender.ToString(),
                    selectedStatus == CohortStatus.WithTransferSender),
                IsAgreementSigned = providerAgreementStatus == ProviderAgreementStatus.Agreed
            };
        }
    }

    public enum CohortStatus
    {
        Unknown,
        Draft,
        Review,
        WithEmployer,
        WithTransferSender
    }
}

using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DetailsViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public bool HasNoDeclaredStandards { get; set; }
        public Party WithParty { get; set; }
        public string CohortReference { get; set; }
        public long CohortId { get; set; }
        public string AccountLegalEntityHashedId { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public string TransferSenderHashedId { get; set; }
        public string EncodedPledgeApplicationId { get; set; }
        public int DraftApprenticeshipsCount
        {
            get
            {
                return Courses?.SelectMany(c => c.DraftApprenticeships).Count() ?? 0;
            }
        }

        public IReadOnlyCollection<DetailsViewCourseGroupingModel> Courses { get; set; }
        public string PageTitle { get; set; }
        public CohortDetailsOptions? Selection { get; set; }
        public string SendMessage { get; set; }
        public string ApproveMessage { get; set; }
        public bool IsApprovedByEmployer{ get; set; }
        public int TotalCost => Courses?.Sum(g => g.DraftApprenticeships.Sum(a => a.Cost ?? 0)) ?? 0;
        public string DisplayTotalCost => TotalCost.ToGdsCostFormat();
        public bool IsAgreementSigned { get; set; }
        public string OptionsTitle => ProviderCanApprove ? "Approve these details?" : "Submit to employer?";
        public bool ShowViewAgreementOption => !IsAgreementSigned;
        public bool ProviderCanApprove => IsAgreementSigned && IsCompleteForProvider && !HasOverlappingUln && !HasEmailOverlaps && !ShowRofjaaRemovalBanner && !ShowInvalidProviderCoursesBanner;
        public bool ShowApprovalOptionMessage => ProviderCanApprove && IsApprovedByEmployer;
        public bool IsReadOnly => WithParty != Party.Provider;
        public bool IsCompleteForProvider { get; set; }
        public bool HasEmailOverlaps { get; set; }
        public bool ShowAddAnotherApprenticeOption { get; set; }
        public bool AllowBulkUpload { get; set; }
        public string SendBackToEmployerOptionMessage
        {
            get
            {
                if (!ProviderCanApprove)
                {
                    return "Yes, send to employer to review or add details";
                }
                return "No, send to employer to review or add details";
            }
        }

        public bool HasOverlappingUln
        {
            get
            {
                return Courses != null
                    && Courses.Count > 0
                    && Courses.Any(x => x.DraftApprenticeships != null
                    && x.DraftApprenticeships.Any(x => x.HasOverlappingUln));
            }
        }

        public bool IsLinkedToChangeOfPartyRequest { get; set; }
        public string Status { get; set; }
        public bool ShowRofjaaRemovalBanner { get; set; }
        public bool ShowInvalidProviderCoursesBanner => InvalidProviderCourseCodes == null? false : InvalidProviderCourseCodes.Any();
        public List<string> InvalidProviderCourseCodes { get; set; }
    }

    public enum CohortDetailsOptions
    {
        Send,
        Approve,
        ApprenticeRequest
    }

}

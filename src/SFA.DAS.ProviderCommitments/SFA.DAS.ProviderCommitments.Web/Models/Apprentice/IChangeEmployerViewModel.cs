using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public interface IChangeEmployerViewModel
    {
    }

    public class InformViewModel : IAuthorizationContextModel, IChangeEmployerViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
        public string LegalEntityName { get; set; }
    }

    public class ChangeEmployerRequestDetailsViewModel : IAuthorizationContextModel, IChangeEmployerViewModel
    {
        public string ApprenticeshipHashedId { get; set; }
        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
        public string EmployerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Price { get; set; }
        public string CurrentEmployerName { get; set; }
        public DateTime CurrentStartDate { get; set; }
        public DateTime CurrentEndDate { get; set; }
        public decimal CurrentPrice { get; set; }
        public long? CohortId { get; set; }
        public string CohortReference { get; set; }
        public Party? WithParty { get; set; }
        public ChangeOfPartyRequestStatus Status { get; set; }
        public string EncodedNewApprenticeshipId { get; set; }
    }
}
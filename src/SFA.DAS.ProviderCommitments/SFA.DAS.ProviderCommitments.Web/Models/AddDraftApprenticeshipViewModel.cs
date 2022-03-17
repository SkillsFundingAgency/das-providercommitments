using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        
        public long AccountLegalEntityId { get; set; }
    }

    public class SelectCourseViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public TrainingProgramme[] Courses { get; set; }
        public string CourseCode { get; set; }
        public string StartMonthYear { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
    }

    public class SelectDeliveryModelViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public Guid? ReservationId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string CourseCode { get; set; }
        public string StartMonthYear { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public DeliveryModel[] DeliveryModels { get; set; }
    }
}
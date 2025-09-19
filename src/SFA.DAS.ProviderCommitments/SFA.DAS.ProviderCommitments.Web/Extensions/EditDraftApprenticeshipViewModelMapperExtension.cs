using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class EditDraftApprenticeshipViewModelMapperExtension
    {
        public static ValidateDraftApprenticeshipApimRequest MapDraftApprenticeship(this DraftApprenticeshipViewModel source)
        {
            return new ValidateDraftApprenticeshipApimRequest
            {
                CohortId = source.CohortId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                EmploymentPrice = source.EmploymentPrice,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                ActualStartDate = source.ActualStartDate.Date,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference,
                ReservationId = source.ReservationId,
                IgnoreStartDateOverlap = true
            };
        }
    }
}

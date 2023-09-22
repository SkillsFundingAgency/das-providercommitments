using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class AddDraftApprenticeshipRequestMapper : IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipApimRequest>
    {
        public Task<AddDraftApprenticeshipApimRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            int? GetCost()
            {
                if (source.IsOnFlexiPaymentPilot is not true) return source.Cost;
                if (source.TrainingPrice is null && source.EndPointAssessmentPrice is null) return null;
                return source.TrainingPrice.GetValueOrDefault() + source.EndPointAssessmentPrice.GetValueOrDefault();
            }

            return Task.FromResult(new AddDraftApprenticeshipApimRequest
            {
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                EmploymentPrice = source.EmploymentPrice,
                Cost = GetCost(),
                TrainingPrice = source.TrainingPrice,
                EndPointAssessmentPrice = source.EndPointAssessmentPrice,
                StartDate = source.StartDate.Date,
                ActualStartDate = source.ActualStartDate.Date,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference,
                DeliveryModel = source.DeliveryModel,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            });
        }
    }
}

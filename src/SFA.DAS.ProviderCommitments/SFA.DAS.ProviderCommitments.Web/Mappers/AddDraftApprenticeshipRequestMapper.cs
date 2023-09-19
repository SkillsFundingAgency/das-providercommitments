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
                Cost = source.IsOnFlexiPaymentPilot.GetValueOrDefault() ? source.TrainingPrice.GetValueOrDefault() + source.EndPointAssessmentPrice.GetValueOrDefault() : source.Cost,
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

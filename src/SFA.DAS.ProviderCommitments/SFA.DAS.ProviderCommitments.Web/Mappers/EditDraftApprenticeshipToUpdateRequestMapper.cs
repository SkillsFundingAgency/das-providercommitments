using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipToUpdateRequestMapper : IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipApimRequest>
    {
        public Task<UpdateDraftApprenticeshipApimRequest> Map(EditDraftApprenticeshipViewModel source) =>
            Task.FromResult(new UpdateDraftApprenticeshipApimRequest
            {
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.IsOnFlexiPaymentPilot.GetValueOrDefault() ? source.TrainingPrice + source.EndPointAssessmentPrice : source.Cost,
                TrainingPrice = source.TrainingPrice,
                EndPointAssessmentPrice = source.EndPointAssessmentPrice,
                StartDate = source.StartDate?.Date,
                ActualStartDate = source.ActualStartDate?.Date,
                EndDate = source.EndDate.Date,
                Reference = source.Reference,
                CourseOption = source.TrainingCourseOption == "-1" ? string.Empty : source.TrainingCourseOption,
                DeliveryModel = source.DeliveryModel.Value,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EmploymentPrice = source.EmploymentPrice,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            });
    }
}

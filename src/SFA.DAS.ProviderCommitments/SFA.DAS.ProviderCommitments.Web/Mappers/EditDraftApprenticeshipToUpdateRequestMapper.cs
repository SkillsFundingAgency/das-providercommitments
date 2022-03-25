using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipToUpdateRequestMapper : IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest>
    {
        public Task<UpdateDraftApprenticeshipRequest> Map(EditDraftApprenticeshipViewModel source) =>
            Task.FromResult(new UpdateDraftApprenticeshipRequest
            {
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                Reference = source.Reference,
                CourseOption = source.TrainingCourseOption == "-1" ? string.Empty : source.TrainingCourseOption,
                DeliveryModel = source.DeliveryModel.Value,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EmploymentPrice = source.EmploymentPrice
            });
    }
}

using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper : IMapper<EditApprenticeshipRequestViewModel, ValidateApprenticeshipForEditRequest>
    {
        public Task<ValidateApprenticeshipForEditRequest> Map(EditApprenticeshipRequestViewModel source)
        {
            var result = new ValidateApprenticeshipForEditRequest
            {
                ApprenticeshipId = source.ApprenticeshipId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                ULN = source.ULN,
                Cost = source.Cost,
                ProviderReference = source.ProviderReference,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                DeliveryModel = source.DeliveryModel,
                TrainingCode = source.CourseCode,
                ProviderId = source.ProviderId,
                Version = source.Version,
                Option = source.Option == "TBC" ? string.Empty : source.Option,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EmploymentPrice = source.EmploymentPrice
            };
            return Task.FromResult(result);
        }
    }
}

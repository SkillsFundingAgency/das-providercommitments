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
                DateOfBirth = source.DateOfBirth.Date,
                ULN = source.ULN,
                Cost = source.Cost,
                ProviderReference = source.ProviderReference,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                TrainingCode = source.CourseCode,
                ProviderId = source.ProviderId
            };
            return Task.FromResult(result);
        }
    }
}

using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class EditApprenticeshipRequestViewModelToValidateEditApprenticeshipRequestMapper : IMapper<EditApprenticeshipRequestViewModel, ValidateEditApprenticeshipRequest>
{
    public Task<ValidateEditApprenticeshipRequest> Map(EditApprenticeshipRequestViewModel source)
    {
        var result = new ValidateEditApprenticeshipRequest
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            DateOfBirth = source.DateOfBirth?.Date,
            ULN = source.ULN,
            CourseCode = source.CourseCode,
            Version = source.Version,
            Option = source.Option,
            Cost = source.Cost,
            StartDate = source.StartDate.Date.Value,
            EndDate = source.EndDate.Date.Value,
            DeliveryModel = (int)source.DeliveryModel,
            ProviderReference = source.ProviderReference,
            EmploymentEndDate = source.EmploymentEndDate?.Date,
            EmploymentPrice = source.EmploymentPrice,
            ChangeCourse = string.Empty,
            ChangeDeliveryModel = string.Empty
        };

        return Task.FromResult(result);
    }
}
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class ConfirmEditApprenticeshipViewModelToConfirmEditRequestMapper : IMapper<ConfirmEditApprenticeshipViewModel, ConfirmEditApprenticeshipRequest>
{
    public Task<ConfirmEditApprenticeshipRequest> Map(ConfirmEditApprenticeshipViewModel source)
    {
        return Task.FromResult(new ConfirmEditApprenticeshipRequest
        {
            ApprenticeshipId = source.ApprenticeshipId,
            ProviderId = source.ProviderId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            DateOfBirth = source.DateOfBirth,
            Cost = (int?)source.Cost,
            ProviderReference = source.ProviderReference,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DeliveryModel = source.DeliveryModel?.ToString(),
            EmploymentEndDate = source.EmploymentEndDate,
            EmploymentPrice = source.EmploymentPrice,
            CourseCode = source.CourseCode,
            Version = source.Version,
            Option = source.Option == "TBC" ? string.Empty : source.Option
        });
    }
}
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmEditApprenticeshipViewModelToEditApiRequestMapper : IMapper<ConfirmEditApprenticeshipViewModel, EditApprenticeshipApiRequest>
    {
        public Task<EditApprenticeshipApiRequest> Map(ConfirmEditApprenticeshipViewModel source)
        {
            return Task.FromResult(new EditApprenticeshipApiRequest
            {
                ApprenticeshipId = source.ApprenticeshipId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth,
                Cost = source.Cost,
                ProviderReference = source.ProviderReference,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                CourseCode = source.CourseCode,
                Version = source.Version,
                Option = source.Option == "N/A" ? string.Empty : source.Option
            });
        }
    }
}

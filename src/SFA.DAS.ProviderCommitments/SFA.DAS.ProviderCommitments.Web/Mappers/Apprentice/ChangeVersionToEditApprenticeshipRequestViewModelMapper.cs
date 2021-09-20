using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeVersionToEditApprenticeshipRequestViewModelMapper : IMapper<ChangeVersionViewModel, EditApprenticeshipRequestViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ChangeVersionToEditApprenticeshipRequestViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<EditApprenticeshipRequestViewModel> Map(ChangeVersionViewModel source)
        {
            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

            await Task.WhenAll(apprenticeshipTask, priceEpisodesTask);

            var apprenticeship = apprenticeshipTask.Result;
            var priceEpisodes = priceEpisodesTask.Result;

            var currentPrice = priceEpisodes.PriceEpisodes.GetPrice();

            var versionResponse = await _commitmentsApiClient.GetTrainingProgrammeVersionByCourseCodeAndVersion(apprenticeship.CourseCode, source.SelectedVersion);

            var newStandardVersion = versionResponse.TrainingProgramme;

            var editRequestViewModel = new EditApprenticeshipRequestViewModel(apprenticeship.DateOfBirth, apprenticeship.StartDate, apprenticeship.EndDate)
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ULN = apprenticeship.Uln,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                Email = apprenticeship.Email,
                Cost = currentPrice,
                CourseCode = apprenticeship.CourseCode,
                TrainingName = apprenticeship.CourseName != newStandardVersion.Name ? newStandardVersion.Name : apprenticeship.CourseName,
                Version = source.SelectedVersion,
                ProviderReference = apprenticeship.ProviderReference,
                ProviderId = source.ProviderId,
                HasOptions = newStandardVersion.Options.Any()
            };

            return editRequestViewModel;
        }
    }
}

using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmDataLockChangesViewModelMapper : IMapper<ConfirmDataLockChangesRequest, ConfirmDataLockChangesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ConfirmDataLockChangesViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ConfirmDataLockChangesViewModel> Map(ConfirmDataLockChangesRequest source)
        {
            var dataLockSummariesTask = _commitmentsApiClient.GetApprenticeshipDatalockSummariesStatus(source.ApprenticeshipId);

            var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            var trainingProgrammesTask = _commitmentsApiClient.GetAllTrainingProgrammes();

            await Task.WhenAll(dataLockSummariesTask, priceEpisodesTask, apprenticeshipTask, trainingProgrammesTask);

            var dataLockSummaries = dataLockSummariesTask.Result;
            var priceEpisodes = priceEpisodesTask.Result;
            var apprenticeship = apprenticeshipTask.Result;
            var trainingProgrammes = trainingProgrammesTask.Result;
            
            var dataLockSummary = dataLockSummaries.MapDataLockSummary(trainingProgrammes);

            var dataLocksPrice = dataLockSummaries
                                  .DataLocksWithCourseMismatch
                                  .Concat(dataLockSummaries.DataLocksWithOnlyPriceMismatch)
                                  .Where(m => m.ErrorCode.HasFlag(DataLockErrorCode.Dlock07));

            return new ConfirmDataLockChangesViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeshipId = source.ApprenticeshipId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.Uln,
                DateOfBirth = apprenticeship.DateOfBirth,
                CourseName = apprenticeship.CourseName,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,
                EmployerName = apprenticeship.EmployerName,
                CourseDataLocks = apprenticeship.MapCourseDataLock(dataLockSummary.DataLockWithCourseMismatch, priceEpisodes.PriceEpisodes),
                PriceDataLocks = priceEpisodes.PriceEpisodes.MapPriceDataLock(dataLocksPrice)
            };
        }      
    }
}

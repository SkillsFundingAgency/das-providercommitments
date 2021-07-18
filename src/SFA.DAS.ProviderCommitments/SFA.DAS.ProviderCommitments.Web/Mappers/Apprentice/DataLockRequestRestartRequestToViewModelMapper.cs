using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DataLockRequestRestartRequestToViewModelMapper : IMapper<DataLockRequestRestartRequest, DataLockRequestRestartViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DataLockRequestRestartRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<DataLockRequestRestartViewModel> Map(DataLockRequestRestartRequest source)
        {
            var dataLockSummariesTask = _commitmentsApiClient
                .GetApprenticeshipDatalockSummariesStatus(source.ApprenticeshipId);

            var apprenticeshipTask = _commitmentsApiClient
                .GetApprenticeship(source.ApprenticeshipId);

            var trainingProgrammesTask = _commitmentsApiClient.GetAllTrainingProgrammes();

            await Task.WhenAll(dataLockSummariesTask, apprenticeshipTask, trainingProgrammesTask);

            var dataLockSummaries = dataLockSummariesTask.Result;

            var dataLock = dataLockSummaries.DataLocksWithCourseMismatch
                .Where(x => x.TriageStatus == TriageStatus.Unknown)
                .OrderBy(x => x.IlrEffectiveFromDate)
                .FirstOrDefault();                

            if (dataLock == null)
                throw new Exception($"No data locks exist that can be restarted for apprenticeship: {source.ApprenticeshipId}");

            var apprenticeship = apprenticeshipTask.Result;

            var trainingProgrammes = trainingProgrammesTask.Result;
            var newProgramme = trainingProgrammes.TrainingProgrammes.Single(m => m.CourseCode == dataLock.IlrTrainingCourseCode);

            return new DataLockRequestRestartViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                AccountHashedId = source.AccountHashedId,
                AccountId = source.AccountId,
                ApprenticeshipId = source.ApprenticeshipId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN  = apprenticeship.Uln,
                CourseName = apprenticeship.CourseName,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,                
                NewCourseCode = newProgramme.CourseCode,
                NewCourseName = newProgramme.Name,
                IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
                IlrEffectiveToDate = dataLock.IlrPriceEffectiveToDate
            };
        }       
    }
}

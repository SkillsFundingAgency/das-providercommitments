using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryHandler : IRequestHandler<GetTrainingCoursesQueryRequest, GetTrainingCoursesQueryResponse>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public GetTrainingCoursesQueryHandler(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<GetTrainingCoursesQueryResponse> Handle(GetTrainingCoursesQueryRequest message, CancellationToken cancellationToken)
        {
            var courses = await GetAllRequiredCourses(message.IncludeFrameworks, cancellationToken);

            if (message.EffectiveDate.HasValue)
            {
                courses = courses.Where(x => x.IsActiveOn(message.EffectiveDate.Value));
            }

            var result = new GetTrainingCoursesQueryResponse
            {
                TrainingCourses = courses.OrderBy(m => m.Name).ToArray()
            };

            return result;
        }

        private Task<IEnumerable<TrainingProgramme>> GetAllRequiredCourses(bool getFramework, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<TrainingProgramme>>>
            {
                getFramework ? GetAll(cancellationToken) : GetStandards(cancellationToken)
            };

            return Task.WhenAll(tasks)
                    .ContinueWith(allTasks => allTasks.Result.SelectMany(task => task), cancellationToken);
        }

        private async Task<IEnumerable<TrainingProgramme>> GetStandards(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await _commitmentsApiClient.GetAllTrainingProgrammeStandards(cancellationToken);
            return response.TrainingProgrammes;
        }

        private async Task<IEnumerable<TrainingProgramme>> GetAll(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await _commitmentsApiClient.GetAllTrainingProgrammes(cancellationToken);
            return response.TrainingProgrammes;
        }
    }
}
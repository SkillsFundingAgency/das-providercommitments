using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryHandler : IRequestHandler<GetTrainingCoursesQueryRequest, GetTrainingCoursesQueryResponse>
    {
        private readonly ITrainingProgrammeApiClient _trainingProgrammeApiClient;


        public GetTrainingCoursesQueryHandler(
            ITrainingProgrammeApiClient trainingProgrammeApiClient)
        {
            _trainingProgrammeApiClient = trainingProgrammeApiClient;
        }

        public async Task<GetTrainingCoursesQueryResponse> Handle(GetTrainingCoursesQueryRequest message, CancellationToken cancellationToken)
        {
            var courses = (IEnumerable<ITrainingProgramme>)(await _trainingProgrammeApiClient.GetTrainingProgrammes());

            if (!message.IncludeFrameworks)
            {
                courses = courses.Where(course => course.ProgrammeType != ProgrammeType.Framework);
            }

            if (message.EffectiveDate.HasValue)
            {
                courses = courses.Where(x => x.IsActiveOn(message.EffectiveDate.Value));
            }

            var result = new GetTrainingCoursesQueryResponse
            {
                TrainingProgrammes = courses.OrderBy(m => m.Title).ToArray()
            };

            return result;
        }
    }
}
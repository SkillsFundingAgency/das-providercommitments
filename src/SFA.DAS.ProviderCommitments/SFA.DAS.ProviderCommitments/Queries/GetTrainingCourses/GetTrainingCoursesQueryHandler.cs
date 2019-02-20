using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses
{
    public sealed class GetTrainingCoursesQueryHandler : IRequestHandler<GetTrainingCoursesQueryRequest, GetTrainingCoursesQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetTrainingCoursesQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetTrainingCoursesQueryResponse> Handle(GetTrainingCoursesQueryRequest message, CancellationToken cancellationToken)
        {
            IEnumerable<ITrainingCourse> courses;
            var standardsTask = _apprenticeshipInfoService.GetStandardsAsync();
            if (!message.IncludeFrameworks)
            {
                courses = (await standardsTask).Standards;
            }
            else
            {
                var getFrameworksTask = _apprenticeshipInfoService.GetFrameworksAsync();
                courses = (await standardsTask).Standards.Union((await getFrameworksTask).Frameworks.Cast<ITrainingCourse>());
            }

            var result = new GetTrainingCoursesQueryResponse();

            if (!message.EffectiveDate.HasValue)
            {
                result.TrainingCourses = courses.OrderBy(m => m.Title).ToList();
            }
            else
            {
                result.TrainingCourses = courses.Where(x => x.IsActiveOn(message.EffectiveDate.Value))
                    .OrderBy(m => m.Title)
                    .ToList();
            }

            return result;
        }
    }
}
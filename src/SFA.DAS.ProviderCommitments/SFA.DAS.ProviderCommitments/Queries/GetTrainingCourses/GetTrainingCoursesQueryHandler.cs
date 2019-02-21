using System;
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
            var courses = await GetAllRequiredCourses(message.IncludeFrameworks, cancellationToken);

            if (message.EffectiveDate.HasValue)
            {
                courses = courses.Where(x => x.IsActiveOn(message.EffectiveDate.Value));
            }

            var result = new GetTrainingCoursesQueryResponse
            {
                TrainingCourses = courses.OrderBy(m => m.Title).ToList()
            };

            return result;
        }

        private Task<IEnumerable<ITrainingCourse>> GetAllRequiredCourses(bool getFramework, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<ITrainingCourse>>> {GetStandards(cancellationToken)};

            if (getFramework)
            {
                tasks.Add(GetFramework(cancellationToken));
            }

            return Task.WhenAll(tasks)
                    .ContinueWith(allTasks => allTasks.Result.SelectMany(task => task), cancellationToken);
        }

        private async Task<IEnumerable<ITrainingCourse>> GetStandards(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            var results = await _apprenticeshipInfoService.GetStandardsAsync();
            return results.Standards;
        }

        private async Task<IEnumerable<ITrainingCourse>> GetFramework(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            var results = await _apprenticeshipInfoService.GetFrameworksAsync();
            return results.Frameworks;
        }
    }
}
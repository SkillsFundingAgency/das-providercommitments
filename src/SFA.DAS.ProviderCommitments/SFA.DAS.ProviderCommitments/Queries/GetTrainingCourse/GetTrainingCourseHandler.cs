using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse
{
    public class GetTrainingCourseHandler : IRequestHandler<GetTrainingCourseRequest, GetTrainingCourseResponse>
    {
        private readonly IValidator<GetTrainingCourseRequest> _validator;

        public GetTrainingCourseHandler(IValidator<GetTrainingCourseRequest> validator)
        {
            _validator = validator;
        }

        public Task<GetTrainingCourseResponse> Handle(GetTrainingCourseRequest request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);

            return Task.FromResult(new GetTrainingCourseResponse
            {
                CourseCode = request.CourseCode,
                CourseName = "** Temp Place Holder **"
            });
        }
    }
}
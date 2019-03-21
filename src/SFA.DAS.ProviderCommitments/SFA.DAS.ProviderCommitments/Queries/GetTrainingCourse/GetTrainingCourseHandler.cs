using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Apprenticeships.Api.Client;

namespace SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse
{
    public class GetTrainingCourseHandler : IRequestHandler<GetTrainingCourseRequest, GetTrainingCourseResponse>
    {
        private readonly IValidator<GetTrainingCourseRequest> _validator;
        private readonly ITrainingProgrammeApiClient _trainingProgrammeApiClient;

        public GetTrainingCourseHandler(IValidator<GetTrainingCourseRequest> validator, ITrainingProgrammeApiClient trainingProgrammeApiClient)
        {
            _validator = validator;
            _trainingProgrammeApiClient = trainingProgrammeApiClient;
        }

        public async Task<GetTrainingCourseResponse> Handle(GetTrainingCourseRequest request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);

            var trainingProgramme = await _trainingProgrammeApiClient.GetTrainingProgramme(request.CourseCode);

            if (trainingProgramme == null)
            {
                throw new Exception($"Could not find training programme for {request.CourseCode}");
            }

            return new GetTrainingCourseResponse
            {
                CourseCode = request.CourseCode,
                CourseName = trainingProgramme.Title
            };
        }
    }
}
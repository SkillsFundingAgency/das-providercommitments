using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestToApiCreateCohortRequestMapper : IMapper<CreateCohortRequest, CreateCohortApimRequest>
    {
        public Task<CreateCohortApimRequest> Map(CreateCohortRequest source)
        {
            return Task.FromResult(new CreateCohortApimRequest
            {
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth,
                Uln = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                EmploymentPrice = source.EmploymentPrice,
                Cost = source.Cost,
                TrainingPrice = source.TrainingPrice,
                EndPointAssessmentPrice = source.EndPointAssessmentPrice,
                StartDate = source.StartDate,
                ActualStartDate = source.ActualStartDate,
                EmploymentEndDate = source.EmploymentEndDate,
                EndDate = source.EndDate,
                OriginatorReference = source.OriginatorReference,
                ReservationId = source.ReservationId,
                IgnoreStartDateOverlap = source.IgnoreStartDateOverlap,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot,
                LearnerDataId = source.LearnerDataId
            });
        }
    }
}

using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.HashingTemp;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestMapper : ICreateCohortRequestMapper
    {
        private readonly IHashingService _publicAccountLegalEntityIdHashingService;

        public CreateCohortRequestMapper(IHashingService publicAccountLegalEntityIdHashingService)
        {
            _publicAccountLegalEntityIdHashingService = publicAccountLegalEntityIdHashingService;
        }

        public CreateCohortRequest Map(AddDraftApprenticeshipViewModel source)
        {
            return new CreateCohortRequest
            {
                AccountLegalEntityId = _publicAccountLegalEntityIdHashingService.DecodeValue(source.AccountLegalEntityPublicHashedId),
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.BirthDate.Date,
                UniqueLearnerNumber = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.FinishDate.Date,
                OriginatorReference = source.Reference
            };
        }
    }
}

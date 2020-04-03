using System;
using System.Threading;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestMapper : IMapper<AddDraftApprenticeshipViewModel, CreateCohortRequest>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public CreateCohortRequestMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<CreateCohortRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            var accountLegalEntity = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId, CancellationToken.None);

            if (accountLegalEntity is null)
            {
                throw new EntityNotFoundException($"AccountLegalEntity {source.AccountLegalEntityId} not found", null);
            }

            return new CreateCohortRequest
            {
                AccountId = accountLegalEntity.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId,
                ReservationId = source.ReservationId.Value,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth.Date,
                UniqueLearnerNumber = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference
            };
        }
    }
}

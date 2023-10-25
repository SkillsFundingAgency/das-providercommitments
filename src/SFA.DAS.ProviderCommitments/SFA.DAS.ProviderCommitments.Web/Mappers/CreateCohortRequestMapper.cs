using System;
using System.Threading;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreateCohortRequestMapper : IMapper<AddDraftApprenticeshipOrRoutePostRequest, CreateCohortRequest>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public CreateCohortRequestMapper(ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<CreateCohortRequest> Map(AddDraftApprenticeshipOrRoutePostRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);

            var accountLegalEntity = await _commitmentsApiClient.GetAccountLegalEntity(cacheItem.AccountLegalEntityId, CancellationToken.None);

            if (accountLegalEntity is null)
            {
                throw new Exception($"AccountLegalEntity {source.AccountLegalEntityId} not found", null);
            }

            return new CreateCohortRequest
            {
                AccountId = accountLegalEntity.AccountId,
                AccountLegalEntityId = cacheItem.AccountLegalEntityId,
                ProviderId = source.ProviderId,
                ReservationId = cacheItem.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                UniqueLearnerNumber = source.Uln,
                CourseCode = source.CourseCode,
                Cost = source.IsOnFlexiPaymentPilot.GetValueOrDefault() ? source.TrainingPrice + source.EndPointAssessmentPrice : source.Cost,
                TrainingPrice = source.TrainingPrice,
                EndPointAssessmentPrice = source.EndPointAssessmentPrice,
                EmploymentPrice = source.EmploymentPrice,
                StartDate = source.StartDate.Date,
                ActualStartDate = source.ActualStartDate.Date,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EndDate = source.EndDate.Date,
                OriginatorReference = source.Reference,
                DeliveryModel = source.DeliveryModel,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            };
        }
    }
}

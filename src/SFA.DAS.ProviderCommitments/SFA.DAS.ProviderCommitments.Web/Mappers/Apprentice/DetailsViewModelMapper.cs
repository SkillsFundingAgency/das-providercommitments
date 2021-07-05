using System;
using System.Collections.Generic;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<DetailsViewModelMapper> _logger;
        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService,
            IAuthorizationService authorizationService,
             ILogger<DetailsViewModelMapper> logger)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _authorizationService = authorizationService;
            _logger = logger;            
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            try
            {                
                var data = await GetApprenticeshipData(source.ApprenticeshipId);
                var dataLockSummaryStatus = data.DataLocks.DataLocks.GetDataLockSummaryStatus();
                
                var allowEditApprentice =
                    (data.Apprenticeship.Status == ApprenticeshipStatus.Live ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.WaitingToStart ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.Paused) &&
                    !data.HasProviderUpdates && 
                    !data.HasEmployerUpdates &&
                    dataLockSummaryStatus == DetailsViewModel.DataLockSummaryStatus.None;

                var pendingChangeOfPartyRequest = data.ChangeOfPartyRequests.ChangeOfPartyRequests.SingleOrDefault(x =>
                    x.OriginatingParty == Party.Provider && x.Status == ChangeOfPartyRequestStatus.Pending);

                var approvedChangeOfPartyRequest = data.ChangeOfPartyRequests.ChangeOfPartyRequests.SingleOrDefault(x =>
                    x.OriginatingParty == Party.Provider && x.Status == ChangeOfPartyRequestStatus.Approved);

                var pendingChangeOfProviderRequest = data.ChangeOfPartyRequests.ChangeOfPartyRequests.SingleOrDefault(x =>
                    x.OriginatingParty == Party.Employer && x.Status == ChangeOfPartyRequestStatus.Pending);

                return new DetailsViewModel
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    Employer = data.Apprenticeship.EmployerName,
                    Reference = _encodingService.Encode(data.Apprenticeship.CohortId, EncodingType.CohortReference),
                    Status = data.Apprenticeship.Status,
                    ConfirmationStatus = data.Apprenticeship.ConfirmationStatus,
                    ShowConfirmationStatus = await _authorizationService.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail),
                    StopDate = data.Apprenticeship.StopDate,
                    AgreementId = _encodingService.Encode(data.Apprenticeship.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                    DateOfBirth = data.Apprenticeship.DateOfBirth,
                    Uln = data.Apprenticeship.Uln,
                    CourseName = data.Apprenticeship.CourseName,
                    StartDate = data.Apprenticeship.StartDate,
                    EndDate = data.Apprenticeship.EndDate,
                    ProviderRef = data.Apprenticeship.ProviderReference,
                    Cost = data.PriceEpisodes.PriceEpisodes.GetPrice(),
                    AllowEditApprentice = allowEditApprentice,
                    HasProviderPendingUpdate = data.HasProviderUpdates,
                    HasEmployerPendingUpdate = data.HasEmployerUpdates,
                    DataLockStatus = dataLockSummaryStatus,
                    AvailableTriageOption = CalcTriageStatus(data.Apprenticeship.HasHadDataLockSuccess, data.DataLocks.DataLocks),
                    IsChangeOfEmployerEnabled = !data.ChangeOfPartyRequests.ChangeOfPartyRequests.Any(x => x.OriginatingParty == Party.Provider && (x.Status == ChangeOfPartyRequestStatus.Approved || x.Status == ChangeOfPartyRequestStatus.Pending)),
                    PauseDate = data.Apprenticeship.PauseDate,
                    CompletionDate = data.Apprenticeship.CompletionDate,
                    HasPendingChangeOfPartyRequest = pendingChangeOfPartyRequest != null,
                    PendingChangeOfPartyRequestWithParty = pendingChangeOfPartyRequest?.WithParty,
                    HasApprovedChangeOfPartyRequest = approvedChangeOfPartyRequest != null,
                    HasPendingChangeOfProviderRequest = pendingChangeOfProviderRequest != null,
                    EncodedNewApprenticeshipId = approvedChangeOfPartyRequest?.NewApprenticeshipId != null
                        ? _encodingService.Encode(approvedChangeOfPartyRequest.NewApprenticeshipId.Value,
                            EncodingType.ApprenticeshipId)
                        : null,
                    HasContinuation = data.Apprenticeship.HasContinuation,
                    EncodedPreviousApprenticeshipId = data.Apprenticeship.ContinuationOfId.HasValue && data.Apprenticeship.PreviousProviderId == source.ProviderId
                        ? _encodingService.Encode(data.Apprenticeship.ContinuationOfId.Value, EncodingType.ApprenticeshipId)
                        : null,
                    EmployerHistory = data.ChangeofEmployerChain?.ChangeOfEmployerChain
                        .Select(coe => new EmployerHistory
                        {
                            EmployerName = coe.EmployerName,
                            FromDate = coe.StartDate.Value,
                            ToDate = coe.StopDate.HasValue ? coe.StopDate.Value : coe.EndDate.Value,
                            HashedApprenticeshipId = _encodingService.Encode(coe.ApprenticeshipId, EncodingType.ApprenticeshipId),
                            ShowLink = source.ApprenticeshipId != coe.ApprenticeshipId
                        }).ToList()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error mapping apprenticeship {source.ApprenticeshipId} to DetailsViewModel");
                throw;
            }
        }

        private static DetailsViewModel.TriageOption CalcTriageStatus(bool hasHadDataLockSuccess, IReadOnlyCollection<DataLock> dataLocks)
        {
            if (!hasHadDataLockSuccess)
            {
                return DetailsViewModel.TriageOption.Update;
            }

            var dataLockErrors = dataLocks.Where(x => x.IsUnresolvedError()).ToList();

            if (dataLockErrors.All(x => x.IsPrice()))
                return  DetailsViewModel.TriageOption.Update;

            if (dataLockErrors.Any(x => x.IsCourseAndPrice()))
                return DetailsViewModel.TriageOption.Restart;

            if (dataLockErrors.All(x => x.IsCourse()))
                return DetailsViewModel.TriageOption.Restart;

            if (dataLockErrors.All(x => x.IsCourseOrPrice()))
                return DetailsViewModel.TriageOption.Both;

            return DetailsViewModel.TriageOption.Update;
        }

        private async Task<(GetApprenticeshipResponse Apprenticeship, 
            GetPriceEpisodesResponse PriceEpisodes, 
            bool HasProviderUpdates, 
            bool HasEmployerUpdates,
            GetDataLocksResponse DataLocks,
            GetChangeOfPartyRequestsResponse ChangeOfPartyRequests,
            GetChangeOfEmployerChainResponse ChangeofEmployerChain)> 
            GetApprenticeshipData(long apprenticeshipId)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(apprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(apprenticeshipId);
            var pendingUpdatesTask = _commitmentApiClient.GetApprenticeshipUpdates(apprenticeshipId, new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest { Status = ApprenticeshipUpdateStatus.Pending });
            var dataLocksTask = _commitmentApiClient.GetApprenticeshipDatalocksStatus(apprenticeshipId);
            var changeOfPartyRequestsTask = _commitmentApiClient.GetChangeOfPartyRequests(apprenticeshipId);
            var changeOfEmployerChainTask = _commitmentApiClient.GetChangeOfEmployerChain(apprenticeshipId);

            await Task.WhenAll(detailsResponseTask, priceEpisodesTask, pendingUpdatesTask, dataLocksTask, changeOfEmployerChainTask, changeOfPartyRequestsTask);
            
            return (detailsResponseTask.Result,
                priceEpisodesTask.Result,
                pendingUpdatesTask.Result.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider),
                pendingUpdatesTask.Result.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Employer),
                dataLocksTask.Result,
                changeOfPartyRequestsTask.Result,
                changeOfEmployerChainTask.Result);
        }
    }
}
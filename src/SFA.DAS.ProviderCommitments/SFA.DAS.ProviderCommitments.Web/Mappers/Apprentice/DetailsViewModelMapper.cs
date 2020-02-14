﻿using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            try
            {
                var data = await GetApprenticeshipData(source.ApprenticeshipId);

                var allowEditApprentice =
                    (data.Apprenticeship.Status == ApprenticeshipStatus.Live ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.WaitingToStart ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.Paused) &&
                    !data.HasProviderUpdates && 
                    !data.HasEmployerUpdates &&
                    data.DataLockStatus == DetailsViewModel.DataLockSummaryStatus.None;

                return new DetailsViewModel
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    Employer = data.Apprenticeship.EmployerName,
                    Reference = _encodingService.Encode(data.Apprenticeship.CohortId, EncodingType.CohortReference),
                    Status = data.Apprenticeship.Status,
                    StopDate = data.Apprenticeship.StopDate,
                    AgreementId = _encodingService.Encode(data.Apprenticeship.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                    DateOfBirth = data.Apprenticeship.DateOfBirth,
                    Uln = data.Apprenticeship.Uln,
                    CourseName = data.Apprenticeship.CourseName,
                    StartDate = data.Apprenticeship.StartDate,
                    EndDate = data.Apprenticeship.EndDate,
                    ProviderRef = data.Apprenticeship.Reference,
                    Cost = data.PriceEpisodes.PriceEpisodes.GetPrice(),
                    AllowEditApprentice = allowEditApprentice,
                    HasProviderPendingUpdate = data.HasProviderUpdates,
                    HasEmployerPendingUpdate = data.HasEmployerUpdates,
                    DataLockStatus = data.DataLockStatus
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<(GetApprenticeshipResponse Apprenticeship, 
            GetPriceEpisodesResponse PriceEpisodes, 
            bool HasProviderUpdates, 
            bool HasEmployerUpdates,
            DetailsViewModel.DataLockSummaryStatus DataLockStatus)> 
            GetApprenticeshipData(long apprenticeshipId)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(apprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(apprenticeshipId);
            var pendingUpdatesTask = _commitmentApiClient.GetApprenticeshipUpdates(apprenticeshipId,
                new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest
                    { Status = ApprenticeshipUpdateStatus.Pending });
            var dataLocksTask = _commitmentApiClient.GetApprenticeshipDatalocksStatus(apprenticeshipId);

            await Task.WhenAll(detailsResponseTask, priceEpisodesTask, pendingUpdatesTask, dataLocksTask);

            var detailsResponse = await detailsResponseTask;
            var priceEpisodes = await priceEpisodesTask;
            var pendingUpdates = await pendingUpdatesTask;
            var dataLocks = (await dataLocksTask).DataLocks;

            var dataLockStatus = GetDataLockSummaryStatus(dataLocks);

            return (detailsResponse, 
                priceEpisodes, 
                pendingUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider),
                pendingUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Employer),
                dataLockStatus);
        }

        private static DetailsViewModel.DataLockSummaryStatus GetDataLockSummaryStatus(IReadOnlyCollection<GetDataLocksResponse.DataLock> dataLocks)
        {
            DetailsViewModel.DataLockSummaryStatus dataLockStatus = DetailsViewModel.DataLockSummaryStatus.None;
            if (dataLocks.Any(x => x.TriageStatus != TriageStatus.Unknown && x.DataLockStatus == Status.Fail && !x.IsResolved))
            {
                dataLockStatus = DetailsViewModel.DataLockSummaryStatus.AwaitingTriage;
            }
            else if (dataLocks.Any(x => x.TriageStatus == TriageStatus.Unknown && x.DataLockStatus == Status.Fail && !x.IsResolved))
            {
                dataLockStatus = DetailsViewModel.DataLockSummaryStatus.HasUnresolvedDataLocks;
            }

            return dataLockStatus;
        }
    }
}
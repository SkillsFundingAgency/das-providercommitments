using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[Route("{providerId}/unapproved")]
public class LearnerController(
    IModelMapper modelMapper,
    ICacheStorageService cacheStorage,
    IOuterApiService outerApiService) : Controller
{
    [HttpGet]
    [Route("add/learners/select", Name = RouteNames.SelectLearnerRecord)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectLearnerRecord(SelectLearnerRecordRequest request)
    {
        var model = await modelMapper.Map<SelectLearnerRecordViewModel>(request);
        return View(model);
    }

    [HttpGet]
    [Route("add/learners/select-multiple", Name = RouteNames.SelectMultipleLearnerRecords)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecords(SelectMultipleLearnerRecordsRequest request)
    {
        var model = await modelMapper.Map<SelectMultipleLearnerRecordsViewModel>(request);

        if (model.ModelValidationErrors != null && model.ModelValidationErrors.Count > 0)
        {
            foreach (var error in model.ModelValidationErrors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        return View(model);
    }

    [HttpPost]
    [Route("add/learners/select-multiple")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecords(SelectMultipleLearnerRecordsPostRequest request)
    {
        var validationResult = await modelMapper.Map<ValidateSelectMultipleLearnerRecordsResult>(request);

        if (validationResult.ValidationErrors?.Any() == true)
        {
            var model = new SelectMultipleLearnerRecordsValidationErrorsRequest
            {
                ProviderId = request.ProviderId,
                CacheKey = request.CacheKey
            };
            return RedirectToAction("SelectMultipleLearnerRecordsValidationErrors", model);
        }

        return RedirectToAction(nameof(SelectMultipleFundingChoice), new { request.ProviderId, request.CacheKey });
    }

    [HttpGet]
    [Route("add/learners/select-multiple/funding")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult SelectMultipleFundingChoice(long providerId, Guid cacheKey)
    {
        return View(new SelectMultipleFundingChoiceViewModel
        {
            ProviderId = providerId,
            CacheKey = cacheKey
        });
    }

    [HttpPost]
    [Route("add/learners/select-multiple/funding")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleFundingChoice(SelectMultipleFundingChoiceViewModel request)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(request.CacheKey);
        if (cacheItem == null)
        {
            return RedirectToAction(nameof(SelectMultipleLearnerRecords), new { request.ProviderId, CacheKey = request.CacheKey });
        }

        if (request.Selection == SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectMultipleFundingChoice.Existing)
        {
            cacheItem.ReservationAssignments = cacheItem.SelectedLearners
                .Select(x => new SelectMultipleReservationAssignment
                {
                    LearnerId = x.Id,
                    UseNewReservation = true
                })
                .ToList();
            await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
            return RedirectToAction(nameof(SelectMultipleReservationAssignment), new { request.ProviderId, CacheKey = request.CacheKey });
        }

        var fundingOptions = await outerApiService.GetAccountFundingOptions(cacheItem.AccountId);
        if (cacheItem.LevyStatus == SFA.DAS.Common.Domain.Types.ApprenticeshipEmployerType.NonLevy &&
            cacheItem.SelectedLearners.Count > fundingOptions.RemainingReservationsCount)
        {
            ModelState.AddModelError("Selection",
                $"You can reserve new funds for {fundingOptions.RemainingReservationsCount} learners. Remove learners or assign existing reservations.");
            return View(request);
        }

        cacheItem.ReservationAssignments = cacheItem.SelectedLearners
            .Select(x => new SelectMultipleReservationAssignment
            {
                LearnerId = x.Id,
                UseNewReservation = true
            })
            .ToList();
        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        return await SaveSelectMultipleLearners(request.ProviderId, request.CacheKey);
    }

    [HttpGet]
    [Route("add/learners/select-multiple/assign")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleReservationAssignment(long providerId, Guid cacheKey)
    {
        var model = await BuildReservationAssignmentModel(providerId, cacheKey);
        return View(model);
    }

    [HttpPost]
    [Route("add/learners/select-multiple/assign")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleReservationAssignment(SelectMultipleReservationAssignmentViewModel request)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(request.CacheKey);
        if (cacheItem == null)
        {
            return RedirectToAction(nameof(SelectMultipleLearnerRecords), new { request.ProviderId, CacheKey = request.CacheKey });
        }

        cacheItem.ReservationAssignments = request.Assignments
            .Select(x => new SelectMultipleReservationAssignment
            {
                LearnerId = x.LearnerId,
                ReservationId = x.ReservationId,
                UseNewReservation = !x.ReservationId.HasValue
            })
            .ToList();
        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        var fundingOptions = await outerApiService.GetAccountFundingOptions(cacheItem.AccountId);
        if (cacheItem.LevyStatus == SFA.DAS.Common.Domain.Types.ApprenticeshipEmployerType.NonLevy &&
            cacheItem.ReservationAssignments.Count(x => x.UseNewReservation) > fundingOptions.RemainingReservationsCount)
        {
            ModelState.AddModelError("Assignments",
                $"You can reserve new funds for {fundingOptions.RemainingReservationsCount} learners. Remove learners or assign existing reservations.");
            return View(await BuildReservationAssignmentModel(request.ProviderId, request.CacheKey));
        }

        return await SaveSelectMultipleLearners(request.ProviderId, request.CacheKey);
    }

    private async Task<SelectMultipleReservationAssignmentViewModel> BuildReservationAssignmentModel(long providerId, Guid cacheKey)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(cacheKey);
        var reservations = await outerApiService.GetSelectMultipleAvailableReservations(
            cacheItem.AccountId,
            cacheItem.AccountLegalEntityId.Value);
        var fundingOptions = await outerApiService.GetAccountFundingOptions(cacheItem.AccountId);
        var selectedReservationIds = cacheItem.ReservationAssignments
            .Where(x => x.ReservationId.HasValue)
            .Select(x => x.ReservationId.Value)
            .ToHashSet();
        var newReservationCount = cacheItem.ReservationAssignments.Count(x => x.UseNewReservation);

        return new SelectMultipleReservationAssignmentViewModel
        {
            ProviderId = providerId,
            CacheKey = cacheKey,
            RemainingNewReservations = cacheItem.LevyStatus == SFA.DAS.Common.Domain.Types.ApprenticeshipEmployerType.Levy
                ? int.MaxValue
                : fundingOptions.RemainingReservationsCount,
            Assignments = cacheItem.SelectedLearners.Select(learner =>
            {
                var current = cacheItem.ReservationAssignments.Single(x => x.LearnerId == learner.Id);
                var options = reservations
                    .Where(reservation =>
                        reservation.StartDate <= learner.StartDate &&
                        reservation.ExpiryDate >= learner.StartDate &&
                        (!selectedReservationIds.Contains(reservation.Id) || current.ReservationId == reservation.Id))
                    .Select(reservation => new SelectMultipleReservationOption
                    {
                        ReservationId = reservation.Id,
                        Text = $"Existing reservation ({reservation.StartDate:MMMM yyyy} to {reservation.ExpiryDate:MMMM yyyy})"
                    })
                    .ToList();

                if (cacheItem.LevyStatus == Common.Domain.Types.ApprenticeshipEmployerType.Levy ||
                    newReservationCount < fundingOptions.RemainingReservationsCount)
                {
                    options.Insert(0, new SelectMultipleReservationOption
                    {
                        Text = "Reserve new funding",
                        IsNew = true
                    });
                }

                return new SelectMultipleReservationAssignmentRow
                {
                    LearnerId = learner.Id,
                    LearnerName = learner.Name,
                    StartDate = learner.StartDate,
                    ReservationId = current.ReservationId,
                    Options = options
                };
            }).ToList()
        };
    }

    private async Task<IActionResult> SaveSelectMultipleLearners(long providerId, Guid cacheKey)
    {
        var request = new SelectMultipleLearnerRecordsPostRequest
        {
            ProviderId = providerId,
            CacheKey = cacheKey
        };
        var validationResult = await modelMapper.Map<ValidateSelectMultipleLearnerRecordsResult>(request);
        if (validationResult.ValidationErrors?.Any() == true)
        {
            return RedirectToAction(nameof(SelectMultipleLearnerRecordsValidationErrors), new { providerId, CacheKey = cacheKey });
        }

        var result = await modelMapper.Map<SelectMultipleAddDraftApprenticeshipsResult>(request);
        return RedirectToAction("Details", "Cohort", new { providerId, result.CohortReference });
    }

    [HttpGet]
    [Route("add/learners/select-multiple-validation-errors", Name = RouteNames.SelectMultipleLearnerRecordsValidationErrors)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsValidationErrors(SelectMultipleLearnerRecordsValidationErrorsRequest request)
    {
        var model = await modelMapper.Map<SelectMultipleLearnerRecordsValidationErrorsViewModel>(request);
        return View(model);
    }

    [HttpGet]
    [Route("add/learners/select-multiple-filter", Name = RouteNames.SelectMultipleLearnerRecordsFilter)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsFilter(SelectMultipleLearnerRecordsFilterRequest request)
    {
        var redirectRequest = await modelMapper.Map<SelectMultipleLearnerRecordsRequest>(request);
        return RedirectToAction("SelectMultipleLearnerRecords", redirectRequest);
    }

    [HttpGet]
    [Route("add/learners/select-multiple-sort", Name = RouteNames.SelectMultipleLearnerRecordsSort)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsSort(SelectMultipleLearnerRecordsSortRequest request)
    {
        var redirectRequest = await modelMapper.Map<SelectMultipleLearnerRecordsRequest>(request);
        return RedirectToAction("SelectMultipleLearnerRecords", redirectRequest);
    }

    [HttpGet]
    [Route("add/learners/select-multiple-add", Name = RouteNames.SelectMultipleLearnerRecordsAdd)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsAdd(SelectMultipleLearnerRecordsAddRequest request)
    {
        var redirectRequest = await modelMapper.Map<SelectMultipleLearnerRecordsRequest>(request);
        return RedirectToAction("SelectMultipleLearnerRecords", redirectRequest);
    }

    [HttpGet]
    [Route("add/learners/select-multiple-remove", Name = RouteNames.SelectMultipleLearnerRecordsRemove)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsRemove(SelectMultipleLearnerRecordsRemoveRequest request)
    {
        var redirectRequest = await modelMapper.Map<SelectMultipleLearnerRecordsRequest>(request);
        return RedirectToAction("SelectMultipleLearnerRecords", redirectRequest);
    }

    [HttpGet]
    [Route("add/learners/select-multiple-remove-all", Name = RouteNames.SelectMultipleLearnerRecordsRemoveAll)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectMultipleLearnerRecordsRemoveAll(SelectMultipleLearnerRecordsRemoveAllRequest request)
    {
        var redirectRequest = await modelMapper.Map<SelectMultipleLearnerRecordsRequest>(request);
        return RedirectToAction("SelectMultipleLearnerRecords", redirectRequest);
    }

    [HttpGet]
    [Route("add/learners/select/{learnerDataId}")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> LearnerSelectedForNewCohort(LearnerSelectedRequest request)
    {
        var model = await modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(request);
        return RedirectToRoute(RouteNames.CreateCohortAndAddFirstApprenticeship, model.CloneBaseValues());
    }

    [HttpGet]
    [Route("add-another/learners/select/{learnerDataId}")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> LearnerToBeAddedToCohort(AddAnotherLearnerSelectedRequest request)
    {
        var model = await modelMapper.Map<ReservationsAddDraftApprenticeshipRequest>(request);
        return RedirectToRoute(RouteNames.DraftApprenticeshipAddAnother, model.CloneBaseValues());
    }
}
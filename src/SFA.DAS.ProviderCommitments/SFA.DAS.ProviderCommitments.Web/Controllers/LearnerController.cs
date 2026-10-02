using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[Route("{providerId}/unapproved")]
public class LearnerController(IModelMapper modelMapper) : Controller
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
        //else continue to create cohort and reservations 

        return RedirectToAction("test", validationResult);
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
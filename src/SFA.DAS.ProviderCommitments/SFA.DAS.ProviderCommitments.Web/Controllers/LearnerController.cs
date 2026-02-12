using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[Route("{providerId}/unapproved")]
public class LearnerController : Controller
{
    private readonly IModelMapper _modelMapper;

    public LearnerController(IMediator mediator,
        IModelMapper modelMapper,
        ILinkGenerator urlHelper)
    {
        _modelMapper = modelMapper;
    }

    [HttpGet]
    [Route("add/learners/select", Name = RouteNames.SelectLearnerRecordAndCreateCohort)]
    [Route("{cohortReference}/add/learners/select", Name = RouteNames.SelectLearnerRecordAndAddToCohort)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectLearnerRecord(SelectLearnerRecordRequest request)
    {
        var model = await _modelMapper.Map<SelectLearnerRecordViewModel>(request);
        return View(model);
    }

    [HttpGet]
    [Route("add/learners/select/{learnerDataId}")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> LearnerSelected(LearnerSelectedRequest request)
    {
        var model = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(request);
        return RedirectToAction("AddDraftApprenticeship", "Cohort", model.CloneBaseValues());
    }
}
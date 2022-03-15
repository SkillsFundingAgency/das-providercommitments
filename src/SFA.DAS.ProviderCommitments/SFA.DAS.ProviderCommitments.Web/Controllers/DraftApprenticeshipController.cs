using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Interfaces;
using IAuthorizationService = SFA.DAS.Authorization.Services.IAuthorizationService;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved/{cohortReference}/apprentices")]
    [DasAuthorize(CommitmentOperation.AccessCohort)]
    public class DraftApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IModelMapper _modelMapper;
        private readonly IEncodingService _encodingService;
        private readonly IAuthorizationService _authorizationService;

        public const string DraftApprenticeDeleted = "Apprentice record deleted";

        public DraftApprenticeshipController(IMediator mediator, ICommitmentsApiClient commitmentsApiClient,
            IModelMapper modelMapper, IEncodingService encodingService,
            SFA.DAS.Authorization.Services.IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _commitmentsApiClient = commitmentsApiClient;
            _modelMapper = modelMapper;
            _encodingService = encodingService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Route("add")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddNewDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
            if (_authorizationService.IsAuthorized(ProviderFeature.DeliveryModel))
            {
                return RedirectToAction(nameof(SelectCourse), request);
            }

            return RedirectToAction(nameof(AddDraftApprenticeship), request);
        }

        [HttpGet]
        [Route("add/select-course")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourse(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);

            await AddLegalEntityAndCoursesToModel(model);

            return View("SelectCourse", model);
        }

        [HttpPost]
        [Route("add/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> SetCourse(AddDraftApprenticeshipViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                return RedirectToAction(nameof(SelectCourse), model);
            }

            var request = await _modelMapper.Map<BaseReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModel), request);
        }

        [HttpGet]
        [Route("add/select-delivery-model")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModel(ReservationsAddDraftApprenticeshipRequest request)
        {
            var models = (await GetProviderCourseDeliveryModels(request.ProviderId, request.CourseCode)).ToArray();

            if (models.Count() > 1)
            {
                var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
                return View("SelectDeliveryModel", model);
            }

            request.DeliveryModel = models.FirstOrDefault();
            return RedirectToAction("AddDraftApprenticeship", request);
        }

        [HttpPost]
        [Route("add/select-delivery-model")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SetDeliveryModel(AddDraftApprenticeshipViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "Please select a delivery model option")});
            }

            var request = await _modelMapper.Map<BaseReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction("AddDraftApprenticeship", request);
        }

        [HttpGet]
        [Route("add-another")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            await AddLegalEntityAndCoursesToModel(model);
            return View(model);
        }

        [HttpPost]
        [Route("add-another")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<AddDraftApprenticeshipRequest>(model);
            request.UserId = User.Upn();

            var response = await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);

            if (string.IsNullOrEmpty(model.CourseCode))
            {
                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });    
            }
            
            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, response.DraftApprenticeshipId);
            
            if (draftApprenticeship.HasStandardOptions)
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(draftApprenticeship.Id, EncodingType.ApprenticeshipId);
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new {model.ProviderId, draftApprenticeshipHashedId , model.CohortReference});
            }
            
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipViewModel model)
        {
            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(model);
            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);

            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value);
            
            if (draftApprenticeship.HasStandardOptions)
            {
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new {model.ProviderId, model.DraftApprenticeshipHashedId, model.CohortReference});
            }
            
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ViewEditDraftApprenticeship(DraftApprenticeshipRequest request)
        {
            try
            {
                var model = await _modelMapper.Map<IDraftApprenticeshipViewModel>(request);

                if (model is EditDraftApprenticeshipViewModel editModel)
                {
                    await AddLegalEntityAndCoursesToModel(editModel);
                    return View("EditDraftApprenticeship", editModel);
                }

                return View("ViewDraftApprenticeship", model as ViewDraftApprenticeshipViewModel);
            }
            catch (Exception e) when (e is DraftApprenticeshipNotFoundException)
            {
                return RedirectToAction("Details", "Cohort", new { request.ProviderId, request.CohortReference });
            }
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectOptions(SelectOptionsRequest request)
        {
            var model = await _modelMapper.Map<ViewSelectOptionsViewModel>(request);

            if (!model.Options.Any())
            {
                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
            }

            return View("SelectStandardOption", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        public async Task<IActionResult> PostSelectOptions(ViewSelectOptionsViewModel model)
        {
            var request = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(model);
            
            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId, model.DraftApprenticeshipId, request);
            
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}")]
        [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
        public async Task<IActionResult> ViewDraftApprenticeship(DraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<ViewDraftApprenticeshipViewModel>(request);

            return View("ViewDraftApprenticeship", model);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/Delete", Name = RouteNames.ApprenticeDelete)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> DeleteConfirmation(DeleteConfirmationRequest deleteConfirmationRequest)
        {
            try
            {
                var viewModel = await _modelMapper.Map<DeleteConfirmationViewModel>(deleteConfirmationRequest);
                return View(viewModel);
            }
            catch (Exception e) when (e is DraftApprenticeshipNotFoundException)
            {
                return RedirectToAction("Details", "Cohort", new { deleteConfirmationRequest.ProviderId, deleteConfirmationRequest.CohortReference });
            }
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/Delete", Name = RouteNames.ApprenticeDelete)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> DeleteConfirmation(DeleteConfirmationViewModel viewModel)
        {
            if (viewModel.DeleteConfirmed != null && !viewModel.DeleteConfirmed.Value)
            {
                return RedirectToAction("ViewEditDraftApprenticeship", "DraftApprenticeship", new DraftApprenticeshipRequest
                {
                    ProviderId = viewModel.ProviderId,
                    CohortReference = viewModel.CohortReference,
                    DraftApprenticeshipHashedId = viewModel.DraftApprenticeshipHashedId
                });
            }

            await _commitmentsApiClient.DeleteDraftApprenticeship(viewModel.CohortId, viewModel.DraftApprenticeshipId, new DeleteDraftApprenticeshipRequest(), CancellationToken.None);
            TempData.AddFlashMessage(DraftApprenticeDeleted, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
        }

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _commitmentsApiClient.GetCohort(model.CohortId.Value);

            var courses = await GetCourses(cohortDetail);

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = courses;
        }

        private async Task<TrainingProgramme[]> GetCourses(GetCohortResponse cohortDetails)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = (!cohortDetails.IsFundedByTransfer &&
                                     cohortDetails.LevyStatus != ApprenticeshipEmployerType.NonLevy)
                                    || cohortDetails.IsLinkedToChangeOfPartyRequest
            });

            return result.TrainingCourses;
        }

        private async Task<IEnumerable<DeliveryModel>> GetProviderCourseDeliveryModels(long providerId, string courseCode)
        {
            var result = await _mediator.Send(new GetProviderCourseDeliveryModelsQueryRequest
            {
                ProviderId = providerId,
                CourseId = courseCode,
            });
            return result.DeliveryModels;
        }
    }
}
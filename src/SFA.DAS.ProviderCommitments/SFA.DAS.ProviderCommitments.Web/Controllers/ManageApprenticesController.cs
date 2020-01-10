using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices")]
    public class ManageApprenticesController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly ICreateCsvService _createCsvService;
        private readonly IMapper<ApprenticeshipDetails, ApprenticeshipDetailsViewModel> _mapper;

        public ManageApprenticesController(
            ICommitmentsService commitmentsService, 
            ICreateCsvService createCsvService,
            IMapper<ApprenticeshipDetails, ApprenticeshipDetailsViewModel> mapper)
        {
            _commitmentsService = commitmentsService;
            _createCsvService = createCsvService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(uint providerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var apprenticeships = await _commitmentsService.GetApprenticeships(providerId);
            var viewModels = new List<ApprenticeshipDetailsViewModel>();
            if (apprenticeships != null)
            {
                foreach (var apprenticeshipDetails in apprenticeships)
                {
                    viewModels.Add(await _mapper.Map(apprenticeshipDetails));
                }
            }
            
            var model = new ManageApprenticesViewModel
            {
                ProviderId = providerId,
                Apprenticeships = viewModels
            };

            return View(model);
        }

        [HttpGet]
        [Route("download",Name = "Download")]
        public async Task<IActionResult> Download(uint providerId)
        {
            var result = await _commitmentsService.GetApprenticeships(providerId);

            var csvContent = result.Select(c => (ApprenticeshipDetailsCsvModel)c).ToList();
            
            var csvFileContent = _createCsvService.GenerateCsvContent(csvContent);
            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv");
        }

        [Route("{apprenticeshipId}", Name = "ApprenticeshipDetails")]
        public IActionResult Details(uint providerId, long apprenticeshipId)
        {
            return Content($"Details of apprenticeship Id:[{apprenticeshipId}].");
        }
    }
}
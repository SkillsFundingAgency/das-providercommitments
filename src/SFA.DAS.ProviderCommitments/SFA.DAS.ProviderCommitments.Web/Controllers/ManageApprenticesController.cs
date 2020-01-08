using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices")]
    public class ManageApprenticesController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly ICreateCsvService _createCsvService;

        public ManageApprenticesController(ICommitmentsService commitmentsService, ICreateCsvService createCsvService)
        {
            _commitmentsService = commitmentsService;
            _createCsvService = createCsvService;
        }

        [Route("manage", Name = RouteNames.ManageApprentices)]
        public async Task<IActionResult> Index(uint providerId, int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _commitmentsService.GetApprenticeships(providerId, pageNumber, ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage);

            var filterModel = new ManageApprenticesFilterModel
            {
                TotalNumberOfApprenticeshipsFound = result.TotalNumberOfApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = result.TotalNumberOfApprenticeshipsWithAlertsFound,
                PageNumber = pageNumber
            };

            var model = new ManageApprenticesViewModel
            {
                ProviderId = providerId,
                Apprenticeships = result.Apprenticeships,
                FilterModel = filterModel
            };

            return View(model);
        }

        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        public async Task<IActionResult> Download(uint providerId)
        {
            var result = await _commitmentsService.GetApprenticeships(providerId);

            var csvContent = result.Apprenticeships.Select(c => (ApprenticeshipDetailsCsvViewModel)c).ToList();
            
            var csvFileContent = _createCsvService.GenerateCsvContent(csvContent);
            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv");
        }
    }
}
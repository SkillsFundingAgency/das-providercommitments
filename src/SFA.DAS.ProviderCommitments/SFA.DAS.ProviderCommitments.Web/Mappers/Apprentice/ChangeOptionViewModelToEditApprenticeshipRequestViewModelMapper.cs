using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeOptionViewModelToEditApprenticeshipRequestViewModelMapper : IMapper<ChangeOptionViewModel, EditApprenticeshipRequestViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public ChangeOptionViewModelToEditApprenticeshipRequestViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IHttpContextAccessor httpContext, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _httpContext = httpContext;
            _tempDataDictionaryFactory = tempDataDictionaryFactory; ;
        }

        public async Task<EditApprenticeshipRequestViewModel> Map(ChangeOptionViewModel source)
        {
            var httpContext = _httpContext.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

            var editViewModel = tempData.GetButDontRemove<EditApprenticeshipRequestViewModel>("EditApprenticeshipRequestViewModel");

            if (editViewModel == null)
            {
                var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
                var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);

                await Task.WhenAll(apprenticeshipTask, priceEpisodesTask);

                var apprenticeship = apprenticeshipTask.Result;
                var priceEpisodes = priceEpisodesTask.Result;

                var currentPrice = priceEpisodes.PriceEpisodes.GetPrice();

                var standardVersion = await _commitmentsApiClient.GetTrainingProgrammeVersionByCourseCodeAndVersion(apprenticeship.CourseCode, apprenticeship.Version);

                editViewModel = new EditApprenticeshipRequestViewModel(apprenticeship.DateOfBirth, apprenticeship.StartDate, apprenticeship.EndDate)
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ULN = apprenticeship.Uln,
                    FirstName = apprenticeship.FirstName,
                    LastName = apprenticeship.LastName,
                    Email = apprenticeship.Email,
                    Cost = currentPrice,
                    DeliveryModel = apprenticeship.DeliveryModel,
                    CourseCode = apprenticeship.CourseCode,
                    Version = apprenticeship.Version,
                    TrainingName = apprenticeship.CourseName,
                    ProviderReference = apprenticeship.ProviderReference,
                    HasOptions = standardVersion.TrainingProgramme.Options.Any()
                };
            }

            editViewModel.Option = source.SelectedOption == "TBC" ? string.Empty : source.SelectedOption;

            return editViewModel;
        }
    }
}

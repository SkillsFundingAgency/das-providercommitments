using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper : IMapper<EditApprenticeshipRequestViewModel, ConfirmEditApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApi;
        private readonly IEncodingService _encodingService;

        public ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper(ICommitmentsApiClient commitmentsApi, IEncodingService encodingService)
        {
            _commitmentApi = commitmentsApi;
            _encodingService = encodingService;
        }

        public async Task<ConfirmEditApprenticeshipViewModel> Map(EditApprenticeshipRequestViewModel source)
        {
            source.ApprenticeshipId = _encodingService.Decode(source.ApprenticeshipHashedId, EncodingType.ApprenticeshipId);
            source.AccountId = _encodingService.Decode(source.AccountHashedId, EncodingType.AccountId);

            var apprenticeshipTask = _commitmentApi.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodesTask = _commitmentApi.GetPriceEpisodes(source.ApprenticeshipId);

            await Task.WhenAll(apprenticeshipTask, priceEpisodesTask);

            var apprenticeship = apprenticeshipTask.Result;
            var priceEpisodes = priceEpisodesTask.Result;

            var currentPrice = priceEpisodes.PriceEpisodes.GetPrice();

            var vm = new ConfirmEditApprenticeshipViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                OriginalApprenticeship = new ConfirmEditApprenticeshipViewModel()
                {
                    ULN = apprenticeship.Uln
                }
            };

            if (source.FirstName != apprenticeship.FirstName || source.LastName != apprenticeship.LastName)
            {
                vm.FirstName = source.FirstName;
                vm.LastName = source.LastName;
            }

            vm.OriginalApprenticeship.FirstName = apprenticeship.FirstName;
            vm.OriginalApprenticeship.LastName = apprenticeship.LastName;

            if (source.DateOfBirth.Date != apprenticeship.DateOfBirth)
            {
                vm.BirthDay = source.BirthDay;
                vm.BirthMonth = source.BirthMonth;
                vm.BirthYear = source.BirthYear;
            }
            vm.OriginalApprenticeship.BirthDay = apprenticeship.DateOfBirth.Day;
            vm.OriginalApprenticeship.BirthMonth = apprenticeship.DateOfBirth.Month;
            vm.OriginalApprenticeship.BirthYear = apprenticeship.DateOfBirth.Year;

            if (source.Cost != currentPrice)
            {
                vm.Cost = source.Cost;
            }
            vm.OriginalApprenticeship.Cost = currentPrice;
            vm.ProviderReference = source.ProviderReference;
            vm.OriginalApprenticeship.ProviderReference = apprenticeship.ProviderReference;

            if (source.StartDate.Date != apprenticeship.StartDate)
            {
                vm.StartMonth = source.StartMonth;
                vm.StartYear = source.StartYear;
            }
            vm.OriginalApprenticeship.StartMonth = apprenticeship.StartDate.Month;
            vm.OriginalApprenticeship.StartYear = apprenticeship.StartDate.Year;

            if (source.EndDate.Date != apprenticeship.EndDate)
            {
                vm.EndMonth = source.EndMonth;
                vm.EndYear = source.EndYear;
            }
            vm.OriginalApprenticeship.EndMonth = apprenticeship.EndDate.Month;
            vm.OriginalApprenticeship.EndYear = apprenticeship.EndDate.Year;

            if (source.CourseCode != apprenticeship.CourseCode)
            {
                var courseDetails = await _commitmentApi.GetTrainingProgramme(source.CourseCode);
                vm.CourseCode = source.CourseCode;
                vm.CourseName = courseDetails?.TrainingProgramme.Name;
            }
            vm.OriginalApprenticeship.CourseCode = apprenticeship.CourseCode;
            vm.OriginalApprenticeship.CourseName = apprenticeship.CourseName;

            return vm;
        }
    }
}

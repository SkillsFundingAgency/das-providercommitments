using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
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

            if (source.Email != apprenticeship.Email)
            {
                vm.Email = source.Email;
            }
            vm.OriginalApprenticeship.Email = apprenticeship.Email;

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
            vm.OriginalApprenticeship.StartMonth = apprenticeship.StartDate.Value.Month;
            vm.OriginalApprenticeship.StartYear = apprenticeship.StartDate.Value.Year;

            if (source.EndDate.Date != apprenticeship.EndDate)
            {
                vm.EndMonth = source.EndMonth;
                vm.EndYear = source.EndYear;
            }
            vm.OriginalApprenticeship.EndMonth = apprenticeship.EndDate.Month;
            vm.OriginalApprenticeship.EndYear = apprenticeship.EndDate.Year;

            if (source.DeliveryModel != apprenticeship.DeliveryModel)
            {
                vm.DeliveryModel = source.DeliveryModel;
            }
            vm.OriginalApprenticeship.DeliveryModel = apprenticeship.DeliveryModel;

            if (source.EmploymentEndDate.Date != apprenticeship.EmploymentEndDate)
            {
                vm.EmploymentEndDate = source.EmploymentEndDate.Date;
            }
            vm.OriginalApprenticeship.EmploymentEndDate = apprenticeship.EmploymentEndDate;

            if (source.EmploymentPrice != apprenticeship.EmploymentPrice)
            {
                vm.EmploymentPrice = source.EmploymentPrice;
            }
            vm.OriginalApprenticeship.EmploymentPrice = apprenticeship.EmploymentPrice;

            if (source.CourseCode != apprenticeship.CourseCode)
            {
                vm.CourseCode = source.CourseCode;
            }
            vm.OriginalApprenticeship.CourseCode = apprenticeship.CourseCode;

            if (source.Version != apprenticeship.Version || source.CourseCode != apprenticeship.CourseCode)
            {
                vm.Version = source.Version;
            }
            vm.OriginalApprenticeship.Version = apprenticeship.Version;

            if (source.TrainingName != apprenticeship.CourseName)
            {
                vm.CourseName = source.TrainingName;
            }
            vm.OriginalApprenticeship.CourseName = apprenticeship.CourseName;

            vm.Option = source.Option == string.Empty ? "TBC" : source.Option;
            vm.OriginalApprenticeship.Option = apprenticeship.Option == string.Empty ? "TBC" : apprenticeship.Option; ;

            if (source.HasOptions)
            {
                vm.ReturnToChangeOption = source.HasOptions;
            }
            else
            {
                vm.ReturnToChangeVersion = !string.IsNullOrEmpty(vm.Version) && string.IsNullOrEmpty(vm.CourseCode) && !vm.StartDate.HasValue;
            }

            return vm;
        }
    }
}

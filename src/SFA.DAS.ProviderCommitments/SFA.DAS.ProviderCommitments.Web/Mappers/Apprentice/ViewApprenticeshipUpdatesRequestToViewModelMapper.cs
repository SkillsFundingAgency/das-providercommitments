using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ViewApprenticeshipUpdatesRequestToViewModelMapper : IMapper<ViewApprenticeshipUpdatesRequest, ViewApprenticeshipUpdatesViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ViewApprenticeshipUpdatesRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ViewApprenticeshipUpdatesViewModel> Map(ViewApprenticeshipUpdatesRequest source)
        {
            var updatesTask = _commitmentsApiClient.GetApprenticeshipUpdates(source.ApprenticeshipId,
                   new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest { Status = CommitmentsV2.Types.ApprenticeshipUpdateStatus.Pending });

            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            await Task.WhenAll(updatesTask, apprenticeshipTask);

            var updates = updatesTask.Result;
            var apprenticeship = apprenticeshipTask.Result;

            if (updates.ApprenticeshipUpdates.Count == 1)
            {
                var update = updates.ApprenticeshipUpdates.First();

                if (!string.IsNullOrWhiteSpace(update.FirstName + update.LastName))
                {
                    update.FirstName = string.IsNullOrWhiteSpace(update.FirstName) ? apprenticeship.FirstName : update.FirstName;
                    update.LastName = string.IsNullOrWhiteSpace(update.LastName) ? apprenticeship.LastName : update.LastName;
                }

                var vm = new ViewApprenticeshipUpdatesViewModel
                {
                    ProviderName = apprenticeship.ProviderName,
                    EmployerName = apprenticeship.EmployerName,
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeshipUpdates = new BaseEdit
                    {
                        FirstName = update.FirstName,
                        LastName = update.LastName,
                        Email = update.Email,
                        DateOfBirth = update.DateOfBirth,
                        Cost = update.Cost,
                        StartDate = update.StartDate,
                        EndDate = update.EndDate,
                        CourseCode = update.TrainingCode,
                        CourseName = update.TrainingName,
                        Version = update.Version,
                        Option = update.Option,
                        DeliveryModel = update.DeliveryModel.Code,
                    },
                    OriginalApprenticeship = new BaseEdit
                    {
                        FirstName = apprenticeship.FirstName,
                        LastName = apprenticeship.LastName,
                        Email = apprenticeship.Email,
                        DateOfBirth = apprenticeship.DateOfBirth,
                        ULN = apprenticeship.Uln,
                        StartDate = apprenticeship.StartDate,
                        EndDate = apprenticeship.EndDate,
                        CourseCode = apprenticeship.CourseCode,
                        CourseName = apprenticeship.CourseName,
                        Version = apprenticeship.Version,
                        Option = apprenticeship.Option,
                        DeliveryModel = apprenticeship.DeliveryModel.Code,
                    }
                };

                if (update.Cost.HasValue)
                {
                    var priceEpisodes = await _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId);
                    vm.OriginalApprenticeship.Cost = priceEpisodes.PriceEpisodes.GetPrice();
                }

                return vm;
            }

            throw new Exception("Multiple pending updates found");
        }
    }
}

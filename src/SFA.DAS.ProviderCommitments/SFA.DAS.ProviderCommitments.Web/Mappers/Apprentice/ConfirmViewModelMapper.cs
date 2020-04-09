using System;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmViewModelMapper : IMapper<ConfirmRequest, ConfirmViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly ILogger<ConfirmViewModelMapper> _logger;
        private readonly ITrainingProgrammeApiClient _trainingProgrammeApiClient;

        public ConfirmViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ITrainingProgrammeApiClient trainingProgrammeApiClient, ILogger<ConfirmViewModelMapper> logger)
        {
            _commitmentApiClient = commitmentsApiClient;
            _trainingProgrammeApiClient = trainingProgrammeApiClient;
            _logger = logger;
        }

        public async Task<ConfirmViewModel> Map(ConfirmRequest source)
        {
            try
            {
                var data = await  GetApprenticeshipData(source.ApprenticeshipId, source.AccountLegalEntityId);

                var newStartDate = new MonthYearModel(source.StartDate);

                return new ConfirmViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    AccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    OldEmployerName = data.Apprenticeship.EmployerName,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    StopDate = data.Apprenticeship.StopDate.Value, 
                    OldStartDate = data.Apprenticeship.StartDate,
                    OldPrice = decimal.ToInt32(data.PriceEpisodes.PriceEpisodes.GetPrice()), 
                    NewEmployerName = data.AccountLegalEntity.LegalEntityName, 
                    NewStartDate = newStartDate,
                    NewPrice = source.Price,
                    FundingBandCap = GetFundingBandCap(data.TrainingProgramme, newStartDate.Date)
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Error mapping apprenticeshipId {source.ApprenticeshipId} to model {nameof(ConfirmViewModel)}", e);
                throw;
            }
        }

        private async Task<(GetApprenticeshipResponse Apprenticeship,
           GetPriceEpisodesResponse PriceEpisodes,
           AccountLegalEntityResponse AccountLegalEntity,
           ITrainingProgramme TrainingProgramme)>
           GetApprenticeshipData(long apprenticeshipId, long newEmployerLegalEntityId)
        {
            var apprenticeship = await _commitmentApiClient.GetApprenticeship(apprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(apprenticeshipId);
            var legalEntityTask =  _commitmentApiClient.GetAccountLegalEntity(newEmployerLegalEntityId);
            var trainingProgrammeTask = _trainingProgrammeApiClient.GetTrainingProgramme(apprenticeship.CourseCode);

            await Task.WhenAll(priceEpisodesTask, legalEntityTask, trainingProgrammeTask);

            var priceEpisodes = await priceEpisodesTask;
            var legalEntity = await legalEntityTask;
            var trainingProgramme = await trainingProgrammeTask;

            return (apprenticeship,
                priceEpisodes,
                legalEntity,
                trainingProgramme);
        }

        private int? GetFundingBandCap(ITrainingProgramme course, DateTime? startDate)
        {
            if (course == null)
            {
                return null;
            }

            var cap = course.FundingCapOn(startDate.Value);

            if (cap > 0)
            {
                return cap;
            }

            return null;
        }
    }
}
using System;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeOfEmployerViewModelMapper : IMapper<ChangeOfEmployerRequest, ChangeOfEmployerViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ILogger<ChangeOfEmployerViewModelMapper> _logger;

        public ChangeOfEmployerViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ILogger<ChangeOfEmployerViewModelMapper> logger)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _logger = logger;
        }

        public async Task<ChangeOfEmployerViewModel> Map(ChangeOfEmployerRequest source)
        {
            try
            {
                var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

                return new ChangeOfEmployerViewModel
                {
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    OldEmployerName = apprenticeship.EmployerName,
                    ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                    StopDate = apprenticeship.StopDate.Value,
                    OldStartDate = apprenticeship.StartDate,
                    OldPrice = 0, // TODO add current apprenticeship cost and return in a field 'apprenticeship.Price or apprenticeship.Cost)
                    NewEmployerName = "New Name", // TODO Determine where to Lookup the new account Name from? Do we call MA API? 
                    NewStartDate = new MonthYearModel(source.StartDate),
                    NewPrice = source.Price
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Error mapping apprenticeshipId {source.ApprenticeshipId} to model {nameof(ChangeOfEmployerViewModel)}", e);
                throw;
            }
        }
    }
}
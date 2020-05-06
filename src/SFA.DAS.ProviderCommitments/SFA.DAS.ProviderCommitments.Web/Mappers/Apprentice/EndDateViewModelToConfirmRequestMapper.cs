using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateViewModelToConfirmRequestMapper : IMapper<EndDateViewModel, ConfirmRequest>
    {
        private readonly ILogger<EndDateViewModelToConfirmRequestMapper> _logger;

        public EndDateViewModelToConfirmRequestMapper(ILogger<EndDateViewModelToConfirmRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ConfirmRequest> Map(EndDateViewModel source)
        {
            try
            {
                return await Task.FromResult(new ConfirmRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    EndDate = source.EndDate.MonthYear,
                    StartDate = source.StartDate,
                    Price = source.Price.Value
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Mapping {nameof(EndDateViewModel)} to {nameof(ConfirmRequest)}", e);
                throw;
            }
        }
    }
}

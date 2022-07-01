using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class StartDateViewModelToConfirmRequestMapper : IMapper<StartDateViewModel, ConfirmRequest>
    {
        private readonly ILogger<StartDateViewModelToConfirmRequestMapper> _logger;

        public StartDateViewModelToConfirmRequestMapper(ILogger<StartDateViewModelToConfirmRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ConfirmRequest> Map(StartDateViewModel source)
        {
            try
            {
                return await Task.FromResult(new ConfirmRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    EndDate = source.EndDate,
                    StartDate = source.StartDate.MonthYear,
                    Price = source.Price.Value,
                    EmploymentEndDate = source.EmploymentEndDate,
                    EmploymentPrice = source.EmploymentPrice ?? 0,
                    CacheKey = source.CacheKey
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Mapping {nameof(StartDateViewModel)} to {nameof(ConfirmRequest)}", e);
                throw;
            }
        }
    }
}

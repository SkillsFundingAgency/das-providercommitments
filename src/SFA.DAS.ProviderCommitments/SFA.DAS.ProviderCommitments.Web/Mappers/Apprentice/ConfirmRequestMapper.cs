using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmRequestMapper : IMapper<PriceViewModel, ConfirmRequest>
    {
        private readonly ILogger<ConfirmRequestMapper> _logger;

        public ConfirmRequestMapper(ILogger<ConfirmRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ConfirmRequest> Map(PriceViewModel source)
        {
            try
            {
                return await Task.FromResult(new ConfirmRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    StartDate = source.StartDate,
                    EndDate = source.EndDate,
                    Price = source.Price.Value,
                    EmploymentEndDate = source.EmploymentEndDate,
                    EmploymentPrice = source.EmploymentPrice,
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Mapping {nameof(PriceViewModel)} to {nameof(ConfirmRequest)}", e);
                throw;
            }
        }
    }
}

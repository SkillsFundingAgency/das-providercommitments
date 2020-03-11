using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeOfEmployerRequestMapper : IMapper<PriceViewModel, ChangeOfEmployerRequest>
    {
        private readonly ILogger<ChangeOfEmployerRequestMapper> _logger;

        public ChangeOfEmployerRequestMapper(ILogger<ChangeOfEmployerRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ChangeOfEmployerRequest> Map(PriceViewModel source)
        {
            try
            {
                return await Task.FromResult(new ChangeOfEmployerRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    StartDate = source.StartDate,
                    Price = decimal.ToInt32(source.Price.Value)
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Mapping {nameof(PriceViewModel)} to {nameof(ChangeOfEmployerRequest)}", e);
                throw;
            }
        }
    }
}

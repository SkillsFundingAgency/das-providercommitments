using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DatesViewModelToConfirmRequestMapper : IMapper<DatesViewModel, ConfirmRequest>
    {
        private readonly ILogger<DatesViewModelToConfirmRequestMapper> _logger;

        public DatesViewModelToConfirmRequestMapper(ILogger<DatesViewModelToConfirmRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ConfirmRequest> Map(DatesViewModel source)
        {
            try
            {
                return await Task.FromResult(new ConfirmRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    StartDate = source.StartDate.MonthYear,
                    Price = source.Price.Value
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error Mapping {nameof(DatesViewModel)} to {nameof(ConfirmRequest)}", e);
                throw;
            }
        }
    }
}

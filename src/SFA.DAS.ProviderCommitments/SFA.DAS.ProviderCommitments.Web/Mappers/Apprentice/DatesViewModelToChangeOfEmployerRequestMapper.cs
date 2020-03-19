using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DatesViewModelToChangeOfEmployerRequestMapper : IMapper<DatesViewModel, ChangeOfEmployerRequest>
    {
        private readonly ILogger<DatesViewModelToChangeOfEmployerRequestMapper> _logger;

        public DatesViewModelToChangeOfEmployerRequestMapper(ILogger<DatesViewModelToChangeOfEmployerRequestMapper> logger)
        {
            _logger = logger;
        }

        public async Task<ChangeOfEmployerRequest> Map(DatesViewModel source)
        {
            try
            {
                return await Task.FromResult(new ChangeOfEmployerRequest
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
                _logger.LogError($"Error Mapping {nameof(DatesViewModel)} to {nameof(ChangeOfEmployerRequest)}", e);
                throw;
            }
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class ApprenticeshipInfoService : IApprenticeshipInfoService
    {
        private readonly ApprenticeshipInfoServiceConfiguration _configuration;
        private readonly IApprenticeshipInfoServiceMapper _mapper;

        public ApprenticeshipInfoService(ApprenticeshipInfoServiceConfiguration configuration, IApprenticeshipInfoServiceMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public ProvidersView GetProvider(long ukPrn)
        {
            var api = new Providers.Api.Client.ProviderApiClient(_configuration.BaseUrl);
            var providers = api.Get(ukPrn);
            return _mapper.MapFrom(providers);
        }
    }
}
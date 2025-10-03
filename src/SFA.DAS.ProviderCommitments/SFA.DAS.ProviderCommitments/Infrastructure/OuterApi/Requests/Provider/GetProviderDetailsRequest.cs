using SFA.DAS.ProviderCommitments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider
{
    public class GetProviderDetailsRequest : IGetApiRequest
    {
        private readonly long _ukprn;

        public GetProviderDetailsRequest(long ukprn)
        {
            _ukprn = ukprn;
        }

        public string GetUrl => $"provideraccounts/providerStatus/{_ukprn}";
    }

    public class GetProviderDetailsResponse
    {
        public ProviderStatusType ProviderStatus { get; set; }
    }

}

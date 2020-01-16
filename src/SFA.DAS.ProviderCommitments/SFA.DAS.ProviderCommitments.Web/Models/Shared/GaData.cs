using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public class GaData
    {
        public string DataLoaded { get; set; } = "dataLoaded";
        public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        public string UkPrn { get; set; }
        public string Vpv { get; set; }
    }
}
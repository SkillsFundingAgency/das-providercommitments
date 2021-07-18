using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DatalockConfirmRestartViewModel : IAuthorizationContextModel
    {
        [JsonIgnore]
        public long ProviderId { get; set; }        
        public string ApprenticeshipHashedId { get; set; }
        public bool? SendRequestToEmployer { get; set; }        
    }
}

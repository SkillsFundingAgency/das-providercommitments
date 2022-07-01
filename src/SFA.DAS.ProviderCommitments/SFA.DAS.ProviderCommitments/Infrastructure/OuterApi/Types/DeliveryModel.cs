using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeliveryModel : byte
    {
        Regular = 0,
        PortableFlexiJob = 1,
        FlexiJobAgency = 2
    }
}

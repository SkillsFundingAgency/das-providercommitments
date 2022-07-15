using System.Text.Json.Serialization;
using SFA.DAS.ProviderCommitments.Attributes;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeliveryModel : byte
    {
        [ReferenceMetadata(Description = "Regular", Hint = "The apprentice will have a single employment contract")]
        Regular = 0,
        [ReferenceMetadata(Description = "Portable flexi-job", Hint = "The apprentice will move between multiple employment contracts")]
        PortableFlexiJob = 1,
        [ReferenceMetadata(Description = "Flexi-job agency", Hint = "The apprentice will have a single employment contract with their flexi-job apprenticeship agency as they move between different host employers")]
        FlexiJobAgency = 2
    }
}

using System;

namespace SFA.DAS.ProviderCommitments.Attributes
{
    public class ReferenceMetadataAttribute : Attribute
    {
        public string Description { get; set; }
        public string Hint { get; set; }
    }
}

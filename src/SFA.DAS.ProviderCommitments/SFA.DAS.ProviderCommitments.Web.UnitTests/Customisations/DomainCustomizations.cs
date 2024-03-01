using AutoFixture.AutoMoq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Customisations
{
    public class DomainCustomizations : CompositeCustomization
    {
        public DomainCustomizations() : base(
            new AutoMoqCustomization { ConfigureMembers = true })
        {
        }
    }
}

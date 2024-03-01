using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Customisations
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute() : base(() =>
        {
            var fixture = new Fixture();

            fixture
                .Customize(new DomainCustomizations())
                .Customize<BindingInfo>(c => c.OmitAutoProperties());

            return fixture;
        })
        {
            
        }
    }
}

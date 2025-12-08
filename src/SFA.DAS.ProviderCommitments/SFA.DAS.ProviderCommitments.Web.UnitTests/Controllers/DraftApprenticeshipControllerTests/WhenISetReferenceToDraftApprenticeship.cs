using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

public class WhenISetReferenceToDraftApprenticeship
{
    private DraftApprenticeshipControllerTestFixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new DraftApprenticeshipControllerTestFixture();
    }

    [Test]
    public async Task Then_The_Reference_Is_Saved_And_Redirected_To_ViewDraftApprenticeship()
    {
        _fixture.SetupReference();

        await _fixture.PostToSetReference();

        _fixture
            .VerifyApiUpdateWithReferenceSet()
            .VerifyRedirectedToEditDraftApprenticeship();
    }
}
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmEmployerViewModelMapperTests
    {
        private ConfirmEmployerViewModelMapperTestsFixture _fixture;

        



        public class ConfirmEmployerViewModelMapperTestsFixture
        {
            public Mock<ICommitmentsApiClient> _commitmentsApiClient;

            public ConfirmEmployerViewModelMapperTestsFixture()
            {
                _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            }
        }
    }
}

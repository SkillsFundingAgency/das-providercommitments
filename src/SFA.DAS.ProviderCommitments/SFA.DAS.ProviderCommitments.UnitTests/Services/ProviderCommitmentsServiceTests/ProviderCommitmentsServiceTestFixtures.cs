using AutoFixture;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Models.ApiModels;
using SFA.DAS.ProviderCommitments.Services;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services.ProviderCommitmentsServiceTests
{
    public class ProviderCommitmentsServiceTestFixtures
    {
        public ProviderCommitmentsServiceTestFixtures()
        {
            var autoFixture = new Fixture();

            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();  
            HashingServiceMock = new Mock<IEncodingService>();
            CohortApiDetail = new CohortApiDetails {AccountLegalEntityId = 1, CohortId = 2, LegalEntityName = "LEN"};
            AddDraftApprenticeshipToCohortRequest = autoFixture.Build<AddDraftApprenticeshipToCohortRequest>().Create();

            Sut = new ProviderCommitmentsService(CommitmentsApiClientMock.Object, HashingServiceMock.Object);
        }

        public AddDraftApprenticeshipToCohortRequest AddDraftApprenticeshipToCohortRequest { get; }
        public CohortApiDetails CohortApiDetail { get; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock { get; }
        public Mock<IEncodingService> HashingServiceMock{ get; }

        public ProviderCommitmentsService Sut;

        public ProviderCommitmentsServiceTestFixtures SetupGetCohortDetailsReturnValue(CohortApiDetails retVal)
        {
            //TODO uncomment when API is updated
            //CommitmentsApiClientMock.Setup(x=>x.GetCohortDetail(It.IsAny<long>())).Returns(retVal);
            return this;
        }

        public ProviderCommitmentsServiceTestFixtures SetupHashingToEncodeInput()
        {
            HashingServiceMock.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns((long id, EncodingType encodingType) => $"ALEX{id}X");
            HashingServiceMock.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long id, EncodingType encodingType) => $"CRX{id}X");
            return this;
        }
    }
}
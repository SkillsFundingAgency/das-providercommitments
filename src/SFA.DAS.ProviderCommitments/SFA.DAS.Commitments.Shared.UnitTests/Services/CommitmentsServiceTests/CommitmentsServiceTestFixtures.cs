using System.Threading;
using AutoFixture;
using Moq;
using SFA.DAS.Commitments.Shared.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.Commitments.Shared.UnitTests.Services.CommitmentsServiceTests
{
    public class CommitmentsServiceTestFixtures
    {
        public CommitmentsServiceTestFixtures()
        {
            var autoFixture = new Fixture();

            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();  
            HashingServiceMock = new Mock<IEncodingService>();
            CohortApiDetail = new GetCohortResponse {CohortId = 2, LegalEntityName = "LEN", ProviderName = "ProviderName", TransferSenderId = 1, WithParty = Party.Employer};
            CohortId = autoFixture.Create<long>();
            AddDraftApprenticeshipRequest = autoFixture.Build<AddDraftApprenticeshipRequest>().Create();
            GetDraftApprenticeshipResponse = autoFixture.Build<GetDraftApprenticeshipResponse>().Create();
            GetApprenticeshipsResponse = autoFixture.Build<GetApprenticeshipsResponse>().Create();

            CommitmentsApiClientMock.Setup(x => x.GetApprenticeships(It.IsAny<GetApprenticeshipsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetApprenticeshipsResponse);

            Sut = new CommitmentsService(CommitmentsApiClientMock.Object, HashingServiceMock.Object);
        }

        public long CohortId { get; }
        public AddDraftApprenticeshipRequest AddDraftApprenticeshipRequest { get; }
        public GetDraftApprenticeshipResponse GetDraftApprenticeshipResponse { get; }
        public GetCohortResponse CohortApiDetail { get; }
        public GetApprenticeshipsResponse GetApprenticeshipsResponse { get; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock { get; }
        public Mock<IEncodingService> HashingServiceMock{ get; }

        public CommitmentsService Sut;

        public CommitmentsServiceTestFixtures SetupGetCohortDetailsReturnValue(GetCohortResponse retVal)
        {
            CommitmentsApiClientMock.Setup(x=>x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(retVal);
            return this;
        }

        public CommitmentsServiceTestFixtures SetupGetDraftApprenticeshipReturnValue(GetDraftApprenticeshipResponse retVal)
        {
            CommitmentsApiClientMock.Setup(x => x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(retVal);
            return this;
        }

        public CommitmentsServiceTestFixtures SetupHashingToEncodeInput()
        {
            HashingServiceMock.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns((long id, EncodingType encodingType) => $"ALEX{id}X");
            HashingServiceMock.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long id, EncodingType encodingType) => $"CRX{id}X");
            HashingServiceMock.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long id, EncodingType encodingType) => $"AX{id}X");
            return this;
        }
    }
}
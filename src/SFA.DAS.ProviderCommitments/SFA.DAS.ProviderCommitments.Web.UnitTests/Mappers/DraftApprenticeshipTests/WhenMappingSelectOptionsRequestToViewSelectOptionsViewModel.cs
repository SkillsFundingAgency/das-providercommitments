using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
    public class WhenMappingSelectOptionsRequestToViewSelectOptionsViewModel
    {
        private GetDraftApprenticeshipResponse _draftApprenticeshipApiResponse;
        private GetStandardOptionsResponse _standardOptionsResponse;
        private SelectOptionsRequest _selectOptionsRequest;
        private ViewStandardOptionsViewModelMapper _mapper;
        private Func<Task<ViewSelectOptionsViewModel>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _draftApprenticeshipApiResponse = fixture.Build<GetDraftApprenticeshipResponse>().Create();
            _standardOptionsResponse = fixture.Build<GetStandardOptionsResponse>().Create();
            _selectOptionsRequest = fixture.Build<SelectOptionsRequest>().Create();
            
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            commitmentsApiClient
                .Setup(x => x.GetStandardOptions(_draftApprenticeshipApiResponse.StandardUId, CancellationToken.None))
                .ReturnsAsync(_standardOptionsResponse);
            commitmentsApiClient
                .Setup(x => x.GetDraftApprenticeship(_selectOptionsRequest.CohortId, _selectOptionsRequest.DraftApprenticeshipId, CancellationToken.None))
                .ReturnsAsync(_draftApprenticeshipApiResponse);

            _mapper = new ViewStandardOptionsViewModelMapper(commitmentsApiClient.Object);

            _act = async () => await _mapper.Map(TestHelper.Clone(_selectOptionsRequest));
        }

        [Test]
        public async Task Then_The_Options_Are_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_standardOptionsResponse.Options.ToList(), result.Options);
        }

        [Test]
        public async Task Then_The_DraftApprenticeshipId_Is_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_selectOptionsRequest.DraftApprenticeshipId, result.DraftApprenticeshipId);
        }

        [Test]
        public async Task Then_The_DraftApprenticeshipHashedId_Is_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_selectOptionsRequest.DraftApprenticeshipHashedId, result.DraftApprenticeshipHashedId);
        }
        
        [Test]
        public async Task Then_The_Cohort_Reference_Is_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_selectOptionsRequest.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task Then_The_Cohort_Id_Is_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_selectOptionsRequest.CohortId, result.CohortId);
        }
        
        [Test]
        public async Task Then_The_Provider_Id_Is_Mapped()
        {
            var result = await _act();
            
            Assert.AreEqual(_selectOptionsRequest.ProviderId, result.ProviderId);
        }
    }
}
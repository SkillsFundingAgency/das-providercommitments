using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class WhenIMapSelectCourseViewModelFromEditApprenticeshipRequestViewModel
    {
        private SelectCourseViewModelFromEditApprenticeshipRequestViewModelMapper _mapper;
        private Mock<ISelectCourseViewModelMapperHelper> _helper;
        private Mock<ICommitmentsApiClient> _client;
        private SelectCourseViewModel _model;
        private GetApprenticeshipResponse _apprenticeshipResponse;
        private GetCohortResponse _cohortResponse;
        private EditApprenticeshipRequestViewModel _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x=>x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartDate).Without(x => x.StartMonth).Without(x => x.StartYear)
                .Without(x => x.EndDate).Without(x => x.EndMonth).Without(x => x.EndYear).Create();
            _model = fixture.Create<SelectCourseViewModel>();
            _apprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            _cohortResponse = fixture.Create<GetCohortResponse>();

            _helper = new Mock<ISelectCourseViewModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool?>())).ReturnsAsync(_model);

            _client = new Mock<ICommitmentsApiClient>();
            _client.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_apprenticeshipResponse);
            _client.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cohortResponse);

            _mapper = new SelectCourseViewModelFromEditApprenticeshipRequestViewModelMapper(_helper.Object, _client.Object);
        }

        [Test]
        public async Task TheParamsArePassedInCorrectly()
        {
            var result = await _mapper.Map(_request);
            _client.Verify(x => x.GetApprenticeship(_request.ApprenticeshipId, CancellationToken.None));
            _client.Verify(x => x.GetCohort(_apprenticeshipResponse.CohortId, CancellationToken.None));
            _helper.Verify(x=>x.Map(_request.CourseCode, _cohortResponse.AccountLegalEntityId, null));
       }

        [Test]
        public async Task ThenModelIsReturned()
        {
            var result = await _mapper.Map(_request);
            Assert.That(result, Is.EqualTo(_model));
        }
    }
}

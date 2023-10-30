using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapCreateCohortWithAddDraftApprenticeshipViewModelToSelectCourseViewModel
    {
        private SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper _mapper;
        private Mock<ISelectCourseViewModelMapperHelper> _helper;
        private SelectCourseViewModel _model;
        private CreateCohortWithDraftApprenticeshipRequest _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
            _model = fixture.Create<SelectCourseViewModel>();

            _helper = new Mock<ISelectCourseViewModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool?>())).ReturnsAsync(_model);

            _mapper = new SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(_helper.Object);
        }

        [Test]
        public async Task TheParamsArePassedInCorrectly()
        {
            var result = await _mapper.Map(_request);
            _helper.Verify(x=>x.Map(_request.CourseCode, _request.AccountLegalEntityId, _request.IsOnFlexiPaymentPilot));
       }

        [Test]
        public async Task ThenModelIsReturned()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_model, result);
        }
    }
}

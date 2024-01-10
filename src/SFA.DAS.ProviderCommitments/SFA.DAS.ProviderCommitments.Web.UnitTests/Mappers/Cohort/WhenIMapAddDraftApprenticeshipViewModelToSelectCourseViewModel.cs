using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort

{
    [TestFixture]
    public class WhenIMapAddDraftApprenticeshipViewModelToSelectCourseViewModel
    {
        private SelectCourseViewModelFromAddDraftApprenticeshipViewModelMapper _mapper;
        private Mock<ISelectCourseViewModelMapperHelper> _helper;
        private SelectCourseViewModel _model;
        private AddDraftApprenticeshipViewModel _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Build<AddDraftApprenticeshipViewModel>().Without(x=>x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartDate).Without(x => x.StartMonth).Without(x => x.StartYear)
                .Without(x => x.EndMonth).Without(x => x.EndYear).Create();
            _model = fixture.Create<SelectCourseViewModel>();

            _helper = new Mock<ISelectCourseViewModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool?>())).ReturnsAsync(_model);

            _mapper = new SelectCourseViewModelFromAddDraftApprenticeshipViewModelMapper(_helper.Object);
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
            Assert.That(result, Is.EqualTo(_model));
        }
    }
}

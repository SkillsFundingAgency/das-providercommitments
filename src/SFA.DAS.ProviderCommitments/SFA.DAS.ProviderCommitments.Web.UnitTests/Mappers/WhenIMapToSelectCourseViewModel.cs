using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Mappers;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers

{
    [TestFixture]
    public class WhenIMapToSelectCourseViewModel
    {
        private SelectCourseViewModelMapperHelper _mapper;
        private Mock<IMediator> _mediatorMock;
        private Mock<ICommitmentsApiClient> _commitmentsClientMock;
        private AccountLegalEntityResponse _ale;
        private GetTrainingCoursesQueryResponse _trainingCourses;
        private int _accountLegalEntityId;
        private string _courseCode;
        private bool? _isOnFlexiPaymentsPilot;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _accountLegalEntityId = fixture.Create<int>();
            _courseCode = fixture.Create<string>();
            _isOnFlexiPaymentsPilot = fixture.Create<bool?>();

            _ale = fixture.Create<AccountLegalEntityResponse>();
            _trainingCourses = fixture.Create<GetTrainingCoursesQueryResponse>();
            var includeFrameworks = _ale.LevyStatus != ApprenticeshipEmployerType.NonLevy; 

            _mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(x =>
                    x.Send(It.Is<GetTrainingCoursesQueryRequest>(p => p.IncludeFrameworks == includeFrameworks),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(_trainingCourses);

            _commitmentsClientMock = new Mock<ICommitmentsApiClient>();
            _commitmentsClientMock.Setup(x => x.GetAccountLegalEntity(_accountLegalEntityId, It.IsAny<CancellationToken>())).ReturnsAsync(_ale);

            _mapper = new SelectCourseViewModelMapperHelper(_commitmentsClientMock.Object, _mediatorMock.Object);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentsPilot);
            Assert.AreEqual(_courseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCoursesAreReturnedCorrectly()
        {
            var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentsPilot);
            Assert.AreEqual(_trainingCourses.TrainingCourses, result.Courses);
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentsPilotIsMappedCorrectly()
        {
            var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentsPilot);
            Assert.AreEqual(_isOnFlexiPaymentsPilot, result.IsOnFlexiPaymentsPilot);
        }
    }
}

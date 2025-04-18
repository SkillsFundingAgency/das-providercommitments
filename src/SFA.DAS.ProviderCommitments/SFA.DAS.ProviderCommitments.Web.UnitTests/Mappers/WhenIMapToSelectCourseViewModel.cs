﻿using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Mappers;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;

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
    private bool? _isOnFlexiPaymentPilot;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _accountLegalEntityId = fixture.Create<int>();
        _courseCode = fixture.Create<string>();
        _isOnFlexiPaymentPilot = fixture.Create<bool?>();

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
        var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentPilot);
        result.CourseCode.Should().Be(_courseCode);
    }

    [Test]
    public async Task ThenCoursesAreReturnedCorrectly()
    {
        var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentPilot);
        result.Courses.Should().BeEquivalentTo(_trainingCourses.TrainingCourses);
    }

    [Test]
    public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
    {
        var result = await _mapper.Map(_courseCode, _accountLegalEntityId, _isOnFlexiPaymentPilot);
        result.IsOnFlexiPaymentPilot.Should().Be(_isOnFlexiPaymentPilot);
    }
}
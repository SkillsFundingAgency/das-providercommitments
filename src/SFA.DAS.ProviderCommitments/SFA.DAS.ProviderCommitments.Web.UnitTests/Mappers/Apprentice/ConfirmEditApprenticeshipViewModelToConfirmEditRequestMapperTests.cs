using System;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class ConfirmEditApprenticeshipViewModelToConfirmEditRequestMapperTests
{
    private ConfirmEditApprenticeshipViewModelToConfirmEditRequestMapper _mapper;
    private ConfirmEditApprenticeshipViewModel _viewModel;
    private Mock<IAuthenticationService> _mockAuthenticationService;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture();
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _mockAuthenticationService.Setup(x => x.UserId).Returns("TestUserId");
        _mockAuthenticationService.Setup(x => x.UserName).Returns("TestUserName");
        _mockAuthenticationService.Setup(x => x.UserEmail).Returns("test@example.com");

        _viewModel = fixture.Build<ConfirmEditApprenticeshipViewModel>()
            .With(x => x.StartMonth, DateTime.Now.Month)
            .With(x => x.StartYear, DateTime.Now.Year)
            .With(x => x.EndMonth, DateTime.Now.Month)
            .With(x => x.EndYear, DateTime.Now.Year)
            .With(x => x.EmploymentEndMonth, DateTime.Now.Month)
            .With(x => x.EmploymentEndYear, DateTime.Now.Year)
            .With(x => x.BirthMonth, DateTime.Now.Month)
            .With(x => x.BirthYear, DateTime.Now.Year)
            .With(x => x.BirthDay, DateTime.Now.Day)
            .Create();

        _mapper = new ConfirmEditApprenticeshipViewModelToConfirmEditRequestMapper(_mockAuthenticationService.Object);
    }

    [Test]
    public async Task ApprenticeshipId_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);
        result.ApprenticeshipId.Should().Be(_viewModel.ApprenticeshipId);
    }

    [Test]
    public async Task FirstName_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.FirstName.Should().Be(_viewModel.FirstName);
    }

    [Test]
    public async Task LastName_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.LastName.Should().Be(_viewModel.LastName);
    }

    [Test]
    public async Task Email_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.Email.Should().Be(_viewModel.Email);
    }

    [Test]
    public async Task Dob_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.DateOfBirth.Should().Be(_viewModel.DateOfBirth ?? DateTime.MinValue);
    }

    [Test]
    public async Task StartDate_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.StartDate.Should().Be(_viewModel.StartDate ?? DateTime.MinValue);
    }

    [Test]
    public async Task EndDate_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.EndDate.Should().Be(_viewModel.EndDate ?? DateTime.MinValue);
    }

    [Test, MoqAutoData]
    public async Task DeliveryModel_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.DeliveryModel.Should().Be(_viewModel.DeliveryModel?.ToString() ?? string.Empty);
    }

    [Test, MoqAutoData]
    public async Task EmploymentEndDate_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.EmploymentEndDate.Should().Be(_viewModel.EmploymentEndDate);
    }

    [Test, MoqAutoData]
    public async Task EmploymentPrice_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.EmploymentPrice.Should().Be(_viewModel.EmploymentPrice);
    }

    [Test]
    public async Task Course_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.CourseCode.Should().Be(_viewModel.CourseCode);
    }

    [Test]
    public async Task Cost_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.Cost.Should().Be((int)(_viewModel.Cost ?? 0));
    }

    [Test]
    public async Task Version_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.Version.Should().Be(_viewModel.Version);
    }

    [Test, MoqAutoData]
    public async Task Option_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.Option.Should().Be(_viewModel.Option == "TBC" ? string.Empty : _viewModel.Option);
    }

    [Test]
    public async Task When_OptionIsTBC_Option_IsMapped()
    {
        _viewModel.Option = "TBC";
        var result = await _mapper.Map(_viewModel);

        result.Option.Should().Be(string.Empty);
    }

    [Test]
    public async Task Reference_IsMapped()
    {
        var result = await _mapper.Map(_viewModel);

        result.ProviderReference.Should().Be(_viewModel.ProviderReference);
    }

    [Test, MoqAutoData]
    public async Task Then_Maps_UserInfo_Correctly()
    {
        // Act
        var result = await _mapper.Map(_viewModel);

        // Assert
        result.UserInfo.Should().NotBeNull();
        result.UserInfo.UserId.Should().Be("TestUserId");
        result.UserInfo.UserDisplayName.Should().Be("TestUserName");
        result.UserInfo.UserEmail.Should().Be("test@example.com");
    }
}
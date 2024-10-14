using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;
using FluentAssertions.Execution;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class EditApprenticeshipRequestViewModelToConfirmEditApprenticeshipViewModelMapperTests
{
    private ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture _fixture;

    [SetUp]
    public void Setup() => _fixture = new ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture();

    [Test]
    public async Task CommitmentApiToGetApprenticeshipIsCalled()
    {
        await _fixture.Map();

        _fixture.VerifyCommitmentApiIsCalled();
    }

    [Test]
    public async Task CommitmentApiToGetPriceEpisodeIsCalled()
    {
        await _fixture.Map();

        _fixture.VerifyPriceEpisodeIsCalled();
    }

    [Test]
    public async Task WhenOnlyProviderReferenceIsChanged()
    {
        _fixture.source.ProviderReference = "ProviderRef";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.EmployerReference.Should().NotBeEquivalentTo(_fixture.source.ProviderReference);
            result.ProviderReference.Should().Be(_fixture.source.ProviderReference);
            result.OriginalApprenticeship.ProviderReference.Should().Be(_fixture.ApprenticeshipResponse.ProviderReference);
        }
    }

    [Test]
    public async Task WhenFirstNameIsChanged()
    {
        _fixture.source.FirstName = "FirstName";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.FirstName.Should().NotBe(_fixture.source.FirstName);
            result.FirstName.Should().Be(_fixture.source.FirstName);
            result.OriginalApprenticeship.FirstName.Should().Be(_fixture.ApprenticeshipResponse.FirstName);
        }
    }

    [Test]
    public async Task WhenLastNameIsChanged()
    {
        _fixture.source.LastName = "LastName";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.LastName.Should().NotBe(_fixture.source.LastName);
            result.LastName.Should().Be(_fixture.source.LastName);
            result.OriginalApprenticeship.LastName.Should().Be(_fixture.ApprenticeshipResponse.LastName);
        }
    }

    [Test]
    public async Task WhenEmailIsChanged()
    {
        _fixture.source.Email = "Email";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.Email.Should().NotBe(_fixture.source.Email);
            result.Email.Should().Be(_fixture.source.Email);
            result.OriginalApprenticeship.Email.Should().Be(_fixture.ApprenticeshipResponse.Email);
        }
    }

    [Test]
    public async Task WhenDobIsChanged()
    {
        _fixture.source.DateOfBirth = new DateModel(new DateTime(2000, 12, 31));

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.DateOfBirth.Day.Should().NotBe(_fixture.source.DateOfBirth.Day);
            result.DateOfBirth.Should().Be(_fixture.source.DateOfBirth.Date);
            result.OriginalApprenticeship.DateOfBirth.Should().Be(_fixture.ApprenticeshipResponse.DateOfBirth);
        }
    }

    [Test]
    public async Task WhenStartDateIsChanged()
    {
        var newStartDate = _fixture.ApprenticeshipResponse.StartDate.Value.AddMonths(-1);
        _fixture.source.StartDate = new MonthYearModel(newStartDate.Month.ToString() + newStartDate.Year);

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.StartDate.Should().NotBe(_fixture.source.StartDate.Date);
            result.StartDate.Should().Be(_fixture.source.StartDate.Date);
            result.OriginalApprenticeship.StartDate.Should().Be(_fixture.ApprenticeshipResponse.StartDate);
        }
    }

    [Test]
    public async Task WhenEndDateIsChanged()
    {
        var newEndDate = _fixture.ApprenticeshipResponse.EndDate.AddMonths(-1);
        _fixture.source.EndDate = new MonthYearModel(newEndDate.Month.ToString() + newEndDate.Year);

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.EndDate.Should().NotBe(_fixture.source.EndDate.Date);
            result.EndDate.Should().Be(_fixture.source.EndDate.Date);
            result.OriginalApprenticeship.EndDate.Should().Be(_fixture.ApprenticeshipResponse.EndDate);
        }
    }

    [Test]
    public async Task WhenCourseIsChanged()
    {
        _fixture.source.CourseCode = "Abc";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.CourseCode.Should().NotBe(_fixture.source.CourseCode);
            result.CourseCode.Should().Be(_fixture.source.CourseCode);
            result.OriginalApprenticeship.CourseCode.Should().Be(_fixture.ApprenticeshipResponse.CourseCode);
        }
    }

    [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
    [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
    public async Task WhenDeliveryModelIsChanged(DeliveryModel original, DeliveryModel changedTo)
    {
        _fixture.ApprenticeshipResponse.DeliveryModel = original;
        _fixture.source.DeliveryModel = changedTo;

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.DeliveryModel.Should().NotBe(_fixture.source.DeliveryModel);
            result.DeliveryModel.Should().Be(_fixture.source.DeliveryModel);
            result.OriginalApprenticeship.DeliveryModel.Should().Be(_fixture.ApprenticeshipResponse.DeliveryModel);
        }
    }

    [Test]
    public async Task WhenEmploymentEndDateIsChanged()
    {
        _fixture.WithPortableFlexiJob();

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.EmploymentEndDate.Should().NotBe(_fixture.source.EmploymentEndDate.Date);
            result.EmploymentEndDate.Should().Be(_fixture.source.EmploymentEndDate.Date);
            result.OriginalApprenticeship.EmploymentEndDate.Should().Be(_fixture.ApprenticeshipResponse.EmploymentEndDate);
        }
    }

    [Test]
    public async Task WhenEmploymentPriceIsChanged()
    {
        _fixture.WithPortableFlexiJob();

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.EmploymentPrice.Should().NotBe(_fixture.source.EmploymentPrice);
            result.EmploymentPrice.Should().Be(_fixture.source.EmploymentPrice);
            result.OriginalApprenticeship.EmploymentPrice.Should().Be(_fixture.ApprenticeshipResponse.EmploymentPrice);
        }
    }

    [Test]
    public async Task WhenVersionIsChanged()
    {
        _fixture.source.Version = "1.1";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.Version.Should().NotBe(_fixture.source.Version);
            result.Version.Should().Be(_fixture.source.Version);
        }
    }

    [Test]
    public async Task WhenCourseCodeIsChangeButVersionIsNotChanged_ThenVersionIsMapped()
    {
        _fixture.source.CourseCode = "123";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.Version.Should().NotBe(_fixture.source.Version);
            result.Version.Should().Be(_fixture.source.Version);
        }
    }

    [Test]
    public async Task WhenOptionIsChanged()
    {
        _fixture.source.Option = "NewOption";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            _fixture.ApprenticeshipResponse.Option.Should().NotBe(_fixture.source.Option);
            result.Option.Should().Be(_fixture.source.Option);
        }
    }

    [Test]
    public async Task When_VersionHasOptions_Then_ReturnToChangeOptionsIsTrue()
    {
        _fixture.source.HasOptions = true;

        var result = await _fixture.Map();

        result.ReturnToChangeOption.Should().BeTrue();
    }

    [Test]
    public async Task When_VersionIsChangedDirectly_Then_ReturnToChangeVersionIsTrue()
    {
        _fixture.source.Version = "NewVersion";

        var result = await _fixture.Map();

        result.ReturnToChangeVersion.Should().BeTrue();
    }

    [Test]
    public async Task When_VersionIsChangedByEditCourse_Then_ReturnToChangeVersionAndOptionAreFalse()
    {
        _fixture.source.Version = "NewVersion";
        _fixture.source.CourseCode = "NewCourseCode";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            result.ReturnToChangeVersion.Should().BeFalse();
            result.ReturnToChangeOption.Should().BeFalse();
        }
    }

    [Test]
    public async Task When_VersionIsChangedByEditStartDate_Then_ReturnToChangeVersionAndOptionAreFalse()
    {
        _fixture.source.Version = "NewVersion";
        _fixture.source.StartDate = new MonthYearModel(DateTime.Now.ToString("MMyyyy"));

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            result.ReturnToChangeVersion.Should().BeFalse();
            result.ReturnToChangeOption.Should().BeFalse();
        }
    }

    [Test]
    public async Task WhenMultipleFieldsAreChanged_TheyAreChanged()
    {
        _fixture.source.CourseCode = "NewCourse";
        _fixture.source.LastName = "NewLastName";

        var result = await _fixture.Map();

        using (new AssertionScope())
        {
            result.LastName.Should().Be(_fixture.source.LastName);
            result.CourseCode.Should().Be(_fixture.source.CourseCode);
        }
    }

    [Test]
    public async Task UnchangedFieldsAreNull()
    {
        _fixture.source.CourseCode = "Course";

        var result = await _fixture.Map();
        using (new AssertionScope())
        {
            result.FirstName.Should().BeNull();
            result.LastName.Should().BeNull();
            result.EndMonth.Should().BeNull();
            result.StartMonth.Should().BeNull();
            result.BirthMonth.Should().BeNull();
        }
    }
}

public class ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture
{
    private readonly Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;

    public readonly GetApprenticeshipResponse ApprenticeshipResponse;
    private readonly GetPriceEpisodesResponse _priceEpisodeResponse;

    private readonly ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper _mapper;
    public readonly EditApprenticeshipRequestViewModel source;
    private ConfirmEditApprenticeshipViewModel _resultViewModel;

    public ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture()
    {
        var autoFixture = new Fixture();

        ApprenticeshipResponse = autoFixture.Build<GetApprenticeshipResponse>()
            .With(x => x.CourseCode, "ABC")
            .With(x => x.Version, "1.0")
            .With(x => x.StartDate, new DateTime(2020, 1, 1))
            .With(x => x.EndDate, new DateTime(2021, 1, 1))
            .With(x => x.DateOfBirth, new DateTime(1990, 1, 1))
            .Without(x => x.EmploymentEndDate)
            .Without(x => x.EmploymentPrice)
            .Create();

        source = new EditApprenticeshipRequestViewModel();
        source.ApprenticeshipId = ApprenticeshipResponse.Id;
        source.CourseCode = ApprenticeshipResponse.CourseCode;
        source.FirstName = ApprenticeshipResponse.FirstName;
        source.LastName = ApprenticeshipResponse.LastName;
        source.Email = ApprenticeshipResponse.Email;
        source.DateOfBirth = new DateModel(ApprenticeshipResponse.DateOfBirth);
        source.Cost = 1000;
        source.ProviderReference = ApprenticeshipResponse.ProviderReference;
        source.StartDate = new MonthYearModel(ApprenticeshipResponse.StartDate.Value.Month.ToString() + ApprenticeshipResponse.StartDate.Value.Year);
        source.EndDate = new MonthYearModel(ApprenticeshipResponse.EndDate.Month.ToString() + ApprenticeshipResponse.EndDate.Year);

        _priceEpisodeResponse = autoFixture.Build<GetPriceEpisodesResponse>()
            .With(x => x.PriceEpisodes, new List<PriceEpisode>
            {
                new() { Cost = 1000, FromDate = DateTime.Now.AddMonths(-1), ToDate = null }
            })
            .Create();

        var standardSummary = autoFixture.Create<TrainingProgramme>();
        standardSummary.EffectiveFrom = new DateTime(2018, 1, 1);
        standardSummary.EffectiveTo = new DateTime(2022, 1, 1);
        standardSummary.FundingPeriods = SetPriceBand(1000);

        _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();

        _mockCommitmentsApiClient.Setup(c => c.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApprenticeshipResponse);
        _mockCommitmentsApiClient.Setup(c => c.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_priceEpisodeResponse);
        _mockCommitmentsApiClient.Setup(t => t.GetTrainingProgrammeVersionByCourseCodeAndVersion(source.CourseCode, source.Version, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTrainingProgrammeResponse
            {
                TrainingProgramme = standardSummary
            });

        var encodingService = new Mock<IEncodingService>();
        encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.ApprenticeshipId)).Returns(ApprenticeshipResponse.Id);
        encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.AccountId)).Returns(ApprenticeshipResponse.EmployerAccountId);

        _mapper = new ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper(_mockCommitmentsApiClient.Object, encodingService.Object);
    }

    public ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture WithPortableFlexiJob()
    {
        ApprenticeshipResponse.DeliveryModel = DeliveryModel.PortableFlexiJob;
        ApprenticeshipResponse.EmploymentPrice = 500;
        ApprenticeshipResponse.EmploymentEndDate = new DateTime(2020, 09, 01);
        return this;
    }

    private static List<TrainingProgrammeFundingPeriod> SetPriceBand(int fundingCap)
    {
        return new List<TrainingProgrammeFundingPeriod>
        {
            new()
            {
                EffectiveFrom = new DateTime(2019, 1, 1),
                EffectiveTo = DateTime.Now.AddMonths(1),
                FundingCap = fundingCap
            }
        };
    }

    public async Task<ConfirmEditApprenticeshipViewModel> Map()
    {
        _resultViewModel = await _mapper.Map(source);
        return _resultViewModel;
    }

    internal void VerifyCommitmentApiIsCalled()
    {
        _mockCommitmentsApiClient.Verify(c => c.GetApprenticeship(ApprenticeshipResponse.Id, It.IsAny<CancellationToken>()), Times.Once());
    }

    internal void VerifyPriceEpisodeIsCalled()
    {
        _mockCommitmentsApiClient.Verify(c => c.GetPriceEpisodes(ApprenticeshipResponse.Id, It.IsAny<CancellationToken>()), Times.Once());
    }
}
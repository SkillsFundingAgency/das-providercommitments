using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

public class WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTests
{
    [Test]
    public async Task LegalEntityNameIsMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyLegalEntityNameIsMappedCorrectly();
    }

    [Test]
    public async Task CohortRefMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyCohortReferenceIsMappedCorrectly();
    }

    [Test]
    public async Task NumberOfFileUploadedApprenticesAreMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyNumberOfApprenticesAreMappedCorrectly();
    }

    [Test]
    public async Task NumberOfExistingApprenticesAreMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().WithDraftApprenticeships().Action();

        //Assert
        fixture.VerifyNumberOfExistingApprenticesAreMappedCorrectly();
    }

    [Test]
    public async Task TotalCostForFileUploadedApprenticesAreMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyTotalCostForFileUploadedApprenticesAreMappedCorrectly();
    }


    [Test]
    public async Task TotalCostForExistingApprenticesAreMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act            
        await fixture.WithDefaultData().WithDraftApprenticeships().Action();

        //Assert
        fixture.TotalCostForExistingApprenticesAreMappedCorrectly();
    }

    [Test]
    public async Task FileUploadedCohortDetailsCountMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyFileUploadedCohortDetailsCountMappedCorrectly();
    }

    [Test]
    public async Task ExistingCohortDetailsCountMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act            
        await fixture.WithDefaultData().WithDraftApprenticeships().Action();

        //Assert
        fixture.VerifyExistingCohortDetailsCountMappedCorrectly();
    }

    [Test]
    public async Task FileUploadedCohortDetailsMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().Action();

        //Assert
        fixture.VerifyFileUploadedCohortDetailsMappedCorrectly();
    }

    [Test]
    public async Task ExistingCohortDetailsMappedCorrectly()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act            
        await fixture.WithDefaultData().WithDraftApprenticeships().Action();

        //Assert
        fixture.VerifyExistingCohortDetailsMappedCorrectly();
    }

    [Test]
    public async Task FundingTextMappedCorrectlyForFileUploadedApprentices()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithPriceDefaultData(2).Action();

        //Assert
        fixture.VerifyFundingTextMappedCorrectlyForFileUploadedApprentices();
    }

    [Test]
    public async Task FundingBandInsetTextMappedCorrectlyForFileUploadedApprentices()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithPriceDefaultData(2).Action();

        //Assert
        fixture.VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentices();
    }

    [Test]
    public async Task FundingBandInsetTextMappedCorrectlyForFileUploadedApprentice()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithPriceDefaultData(1).Action();

        //Assert
        fixture.VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentice();
    }

    [Test]
    public async Task FundingTextMappedCorrectlyForExistingApprentices()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().WithExistingCohortPriceDefaultData(3).Action();

        //Assert
        fixture.FundingTextMappedCorrectlyForExistingApprentices();
    }

    [Test]
    public async Task FundingBandInsetTextMappedCorrectlyForExistingApprentices()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().WithExistingCohortPriceDefaultData(3).Action();

        //Assert
        fixture.FundingBandInsetTextMappedCorrectlyForExistingApprentices();
    }

    [Test]
    public async Task FundingBandInsetTextMappedCorrectlyForExistingApprentice()
    {
        //Arrange
        var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

        //Act
        await fixture.WithDefaultData().WithExistingCohortPriceDefaultData(1).Action();

        //Assert
        fixture.FundingBandInsetTextMappedCorrectlyForExistingApprentice();
    }
}

public class WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture
{
    private readonly Fixture _fixture;
    private ReviewApprenticeRequestToReviewApprenticeViewModelMapper _sut;
    private FileUploadReviewApprenticeRequest _request;
    private Mock<IEncodingService> _encodingService;
    private Mock<IOuterApiService> _commitmentApiClient;
    private Mock<ICacheService> _cacheService;
    private List<CsvRecord> _csvRecords;
    private FileUploadCacheModel _fileUploadCacheModel;
    private FileUploadReviewApprenticeViewModel _result;
    private List<GetStandardFundingResponse> _fundingPeriods;
    private GetStandardResponse _trainingProgramme;
    private DateTime _startFundingPeriod = new(2020, 10, 1);
    private DateTime _endFundingPeriod = new(2020, 10, 30);
    public DateTime DefaultStartDate = new(2020, 10, 1);
    private const string cohortRef = "Cohort4";
    private const string dateOfBirth = "2001-09-05";
    private GetDraftApprenticeshipsResult _draftApprenticeshipsResponse;

    public WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture()
    {
        _fixture = new Fixture();
        _csvRecords = new List<CsvRecord>();

        _request = _fixture.Create<FileUploadReviewApprenticeRequest>();
        _fileUploadCacheModel = new FileUploadCacheModel
        {
            CsvRecords = _csvRecords,
            FileUploadLogId = 1235
        };

        _request.CohortRef = cohortRef;
        var accountLegalEntityEmployer = _fixture.Build<GetAccountLegalEntityQueryResult>()
            .With(x => x.LegalEntityName, "EmployerName").Create();

        var cohort = _fixture.Build<GetCohortResult>()
            .With(x => x.LatestMessageCreatedByEmployer, "A message").Create();

        _encodingService = new Mock<IEncodingService>();
        _encodingService.Setup(x => x.Decode("Employer", EncodingType.PublicAccountLegalEntityId)).Returns(1);

        _commitmentApiClient = new Mock<IOuterApiService>();
        _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(1)).ReturnsAsync(accountLegalEntityEmployer);
        _commitmentApiClient.Setup(x => x.GetCohort(0)).ReturnsAsync(cohort);

        _fundingPeriods = new List<GetStandardFundingResponse>
        {
            new() { EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, MaxEmployerLevyCap = 1000 },
            new() { EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), MaxEmployerLevyCap = 500 }
        };
        _trainingProgramme = new GetStandardResponse { Title = "CourseName", EffectiveFrom = DefaultStartDate, EffectiveTo = DefaultStartDate.AddYears(1), ApprenticeshipFunding = _fundingPeriods };

        _commitmentApiClient.Setup(x => x.GetStandardDetails(It.Is<string>(c => !string.IsNullOrEmpty(c))))
            .ReturnsAsync(_trainingProgramme);

        _cacheService = new Mock<ICacheService>();
        _cacheService.Setup(x => x.GetFromCache<FileUploadCacheModel>(_request.CacheRequestId.ToString())).ReturnsAsync(_fileUploadCacheModel);

        _sut = new ReviewApprenticeRequestToReviewApprenticeViewModelMapper(Mock.Of<ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper>>(), _commitmentApiClient.Object, _cacheService.Object, _encodingService.Object);
    }

    public async Task Action() => _result = await _sut.Map(_request);

    internal void VerifyLegalEntityNameIsMappedCorrectly()
    {
        _result.LegalEntityName.Should().Be("EmployerName");
    }

    internal void VerifyCohortReferenceIsMappedCorrectly()
    {
        _result.CohortRef.Should().Be(cohortRef);
    }

    internal void VerifyNumberOfApprenticesAreMappedCorrectly()
    {
        var groupedByCohort = _csvRecords.Where(x => x.CohortRef == cohortRef);

        groupedByCohort.Count().Should().Be(2);
    }

    internal void VerifyNumberOfExistingApprenticesAreMappedCorrectly()
    {
        _draftApprenticeshipsResponse.DraftApprenticeships.Count.Should().Be(3);
    }

    internal void VerifyTotalCostForFileUploadedApprenticesAreMappedCorrectly()
    {
        var groupedByCohort = _csvRecords.Where(x => x.CohortRef == cohortRef);

        groupedByCohort.Sum(x => int.Parse(x.TotalPrice)).Should().Be(1000);
    }

    internal void TotalCostForExistingApprenticesAreMappedCorrectly()
    {
        var costfromfileUploadedCohort = _csvRecords.Where(x => x.CohortRef == cohortRef).Sum(x => int.Parse(x.TotalPrice));
        var costfromExistingCohort = _draftApprenticeshipsResponse.DraftApprenticeships.Sum(x => x.Cost);

        (costfromfileUploadedCohort + costfromExistingCohort).Should().Be(1300);
    }

    internal void VerifyFileUploadedCohortDetailsCountMappedCorrectly()
    {
        _result.FileUploadCohortDetails.Count.Should().Be(2);
    }

    internal void VerifyExistingCohortDetailsCountMappedCorrectly()
    {
        _result.ExistingCohortDetails.Count.Should().Be(3);
    }

    internal void VerifyFileUploadedCohortDetailsMappedCorrectly()
    {
        var csvRecord = _csvRecords.FirstOrDefault(x => x.CohortRef == cohortRef);
        var cohortDetails = _result.FileUploadCohortDetails[0];

        using (new AssertionScope())
        {
            $"{csvRecord.GivenNames} {csvRecord.FamilyName}".Should().Be(cohortDetails.Name);
            csvRecord.EmailAddress.Should().Be(cohortDetails.Email);
            csvRecord.ULN.Should().Be(cohortDetails.ULN);
            csvRecord.DateOfBirth.Should().Be(dateOfBirth);
            decimal.Parse(csvRecord.TotalPrice).Should().Be(cohortDetails.Price);
            _trainingProgramme.Title.Should().Be(cohortDetails.TrainingCourse);
            _trainingProgramme.ApprenticeshipFunding.FirstOrDefault().MaxEmployerLevyCap.Should().Be(cohortDetails.FundingBandCap);
        }
    }

    internal void VerifyExistingCohortDetailsMappedCorrectly()
    {
        var existingRecord = _draftApprenticeshipsResponse.DraftApprenticeships.FirstOrDefault();
        var cohortDetails = _result.ExistingCohortDetails[0];

        using (new AssertionScope())
        {
            $"{existingRecord.FirstName} {existingRecord.LastName}".Should().Be(cohortDetails.Name);
            existingRecord.Email.Should().Be(cohortDetails.Email);
            existingRecord.Uln.Should().Be(cohortDetails.ULN);
            existingRecord.Cost.Value.Should().Be((int)cohortDetails.Price);
            existingRecord.CourseName.Should().Be(cohortDetails.TrainingCourse);
            _trainingProgramme.ApprenticeshipFunding.FirstOrDefault().MaxEmployerLevyCap.Should().Be(cohortDetails.FundingBandCapForExistingCohort);
        }
    }

    internal void VerifyFundingTextMappedCorrectlyForFileUploadedApprentices()
    {
        _result.FundingBandTextForFileUploadCohorts.Should().Be("2 apprenticeships above funding band maximum");
    }

    internal void VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentices()
    {
        _result.FundingBandInsetTextForFileUploadCohorts.Should().Be("The price for these apprenticeships is above the");
    }

    internal void VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentice()
    {
        _result.FundingBandInsetTextForFileUploadCohorts.Should().Be("The price for this apprenticeship is above its");
    }

    internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithDefaultData()
    {
        // This will create three csv records, with total cost of 1000 (2 * 500)
        _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer", cohortRef, dateOfBirth, "2020-10-01", "2022-11", 500, 2));

        return this;
    }

    internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithPriceDefaultData(int numberOfApprentices)
    {
        _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer", cohortRef, dateOfBirth, "2020-10-01", "2022-11", 1500, numberOfApprentices));

        return this;
    }

    private static List<CsvRecord> CreateCsvRecords(Fixture fixture, string employerAgreementId, string cohortRef, string dateOfBirth,
        string apprenticeStartDate, string apprenticeEndDate, int price, int numberOfApprentices)
    {
        var list = fixture.Build<CsvRecord>()
            .With(x => x.AgreementId, employerAgreementId)
            .With(x => x.CohortRef, cohortRef)
            .With(x => x.TotalPrice, price.ToString())
            .With(x => x.DateOfBirth, dateOfBirth)
            .With(x => x.StartDate, apprenticeStartDate)
            .With(x => x.EndDate, apprenticeEndDate)
            .CreateMany(numberOfApprentices)
            .ToList();
        return list;
    }

    internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithDraftApprenticeships()
    {
        _draftApprenticeshipsResponse = new GetDraftApprenticeshipsResult
        {
            DraftApprenticeships = _fixture.Build<Infrastructure.OuterApi.Responses.DraftApprenticeship>()
                .With(x => x.Cost, 100)
                .With(x => x.CourseName, "CourseName")
                .With(x => x.StartDate, DefaultStartDate)
                .CreateMany(3)
                .ToList()
        };
        _commitmentApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>()))
            .ReturnsAsync(_draftApprenticeshipsResponse);

        return this;
    }

    internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithExistingCohortPriceDefaultData(int numberOfApprentices)
    {
        _draftApprenticeshipsResponse = new GetDraftApprenticeshipsResult
        {
            DraftApprenticeships = _fixture.Build<Infrastructure.OuterApi.Responses.DraftApprenticeship>()
                .With(x => x.Cost, 3000)
                .With(x => x.CourseName, "CourseName")
                .With(x => x.StartDate, DefaultStartDate)
                .CreateMany(numberOfApprentices)
                .ToList()
        };
        _commitmentApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>()))
            .ReturnsAsync(_draftApprenticeshipsResponse);

        return this;
    }

    internal void FundingTextMappedCorrectlyForExistingApprentices()
    {
        _result.FundingBandTextForExistingCohorts.Should().Be("3 apprenticeships above funding band maximum");
    }

    internal void FundingBandInsetTextMappedCorrectlyForExistingApprentices()
    {
        _result.FundingBandInsetTextForExistingCohorts.Should().Be("The price for these apprenticeships is above the");
    }

    internal void FundingBandInsetTextMappedCorrectlyForExistingApprentice()
    {
        _result.FundingBandInsetTextForExistingCohorts.Should().Be("The price for this apprenticeship is above its");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
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
        private readonly ReviewApprenticeRequestToReviewApprenticeViewModelMapper _sut;
        private readonly FileUploadReviewApprenticeRequest _request;
        private readonly Mock<IOuterApiService> _commitmentApiClient;
        private readonly List<CsvRecord> _csvRecords;
        private FileUploadReviewApprenticeViewModel _result;
        private readonly GetStandardResponse _trainingProgramme;
        private readonly DateTime _startFundingPeriod = new(2020, 10, 1);
        private readonly DateTime _endFundingPeriod = new(2020, 10, 30);
        private readonly DateTime _defaultStartDate = new(2020, 10, 1);
        private const string CohortRef = "Cohort4";
        private const string DateOfBirth = "2001-09-05";
        private GetDraftApprenticeshipsResult _draftApprenticeshipsResponse;

        public WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture()
        {
            _fixture = new Fixture();
            _csvRecords = new List<CsvRecord>();

            _request = _fixture.Create<FileUploadReviewApprenticeRequest>();
            _request.CohortRef = CohortRef;
            var accountLegalEntityEmployer = _fixture.Build<GetAccountLegalEntityQueryResult>()
                .With(x => x.LegalEntityName, "EmployerName").Create();

            var cohort = _fixture.Build<GetCohortResult>()
                .With(x => x.LatestMessageCreatedByEmployer, "A message").Create();

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Decode("Employer", EncodingType.PublicAccountLegalEntityId)).Returns(1);
            
            _commitmentApiClient = new Mock<IOuterApiService>();
            _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(1)).ReturnsAsync(accountLegalEntityEmployer);
            _commitmentApiClient.Setup(x => x.GetCohort(0)).ReturnsAsync(cohort);

            var fundingPeriods = new List<GetStandardFundingResponse>
            {
                new() { EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, MaxEmployerLevyCap = 1000},
                new() { EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), MaxEmployerLevyCap = 500}
            };
            _trainingProgramme = new GetStandardResponse { Title = "CourseName", EffectiveFrom = _defaultStartDate, EffectiveTo = _defaultStartDate.AddYears(1), ApprenticeshipFunding = fundingPeriods };

            _commitmentApiClient.Setup(x => x.GetStandardDetails(It.Is<string>(c => !string.IsNullOrEmpty(c))))
                .ReturnsAsync(_trainingProgramme);

            var cacheService = new Mock<ICacheService>();
            cacheService.Setup(x => x.GetFromCache<List<CsvRecord>>(_request.CacheRequestId.ToString())).ReturnsAsync(_csvRecords);

            _sut = new ReviewApprenticeRequestToReviewApprenticeViewModelMapper(Mock.Of<ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper>>(), _commitmentApiClient.Object, cacheService.Object, encodingService.Object);
        }

        public async Task Action() => _result = await _sut.Map(_request);

        internal void VerifyLegalEntityNameIsMappedCorrectly()
        {            
            Assert.AreEqual("EmployerName", _result.LegalEntityName);
        }

        internal void VerifyCohortReferenceIsMappedCorrectly()
        {
            Assert.AreEqual(CohortRef, _result.CohortRef);
        }
        
        internal void VerifyNumberOfApprenticesAreMappedCorrectly()
        {
            var groupedByCohort = _csvRecords.Where(x => x.CohortRef == CohortRef);
            
            Assert.AreEqual(2, groupedByCohort.Count());
        }

        internal void VerifyNumberOfExistingApprenticesAreMappedCorrectly()
        {
            Assert.AreEqual(3, _draftApprenticeshipsResponse.DraftApprenticeships.Count());
        }

        internal void VerifyTotalCostForFileUploadedApprenticesAreMappedCorrectly()
        {
            var groupedByCohort = _csvRecords.Where(x => x.CohortRef == CohortRef);

            Assert.AreEqual(1000, groupedByCohort.Sum(x => int.Parse(x.TotalPrice)));
        }

        internal void TotalCostForExistingApprenticesAreMappedCorrectly()
        {
            var costfromfileUploadedCohort = _csvRecords.Where(x => x.CohortRef == CohortRef).Sum(x => int.Parse(x.TotalPrice));
            var costfromExistingCohort = _draftApprenticeshipsResponse.DraftApprenticeships.Sum(x => x.Cost);            

            Assert.AreEqual(1300, (costfromfileUploadedCohort + costfromExistingCohort));
        }

        internal void VerifyFileUploadedCohortDetailsCountMappedCorrectly()
        { 
            Assert.AreEqual(2, _result.FileUploadCohortDetails.Count());
        }

        internal void VerifyExistingCohortDetailsCountMappedCorrectly()
        {
            Assert.AreEqual(3, _result.ExistingCohortDetails.Count());
        }

        internal void VerifyFileUploadedCohortDetailsMappedCorrectly()
        {
            var csvRecord = _csvRecords.FirstOrDefault(x => x.CohortRef == CohortRef);
            var cohortDetails = _result.FileUploadCohortDetails[0];

            Assert.AreEqual(cohortDetails.Name, $"{csvRecord.GivenNames} {csvRecord.FamilyName}");
            Assert.AreEqual(cohortDetails.Email, csvRecord.EmailAddress);
            Assert.AreEqual(cohortDetails.ULN, csvRecord.ULN);
            Assert.AreEqual(DateOfBirth, csvRecord.DateOfBirth);
            Assert.AreEqual(cohortDetails.Price, int.Parse(csvRecord.TotalPrice));
            Assert.AreEqual(cohortDetails.TrainingCourse, _trainingProgramme.Title);
            Assert.AreEqual(cohortDetails.FundingBandCap, _trainingProgramme.ApprenticeshipFunding.FirstOrDefault().MaxEmployerLevyCap);

        }

        internal void VerifyExistingCohortDetailsMappedCorrectly()
        {
            var existingRecord = _draftApprenticeshipsResponse.DraftApprenticeships.FirstOrDefault();
            var cohortDetails = _result.ExistingCohortDetails[0];

            Assert.AreEqual(cohortDetails.Name, $"{existingRecord.FirstName} {existingRecord.LastName}");
            Assert.AreEqual(cohortDetails.Email, existingRecord.Email);
            Assert.AreEqual(cohortDetails.ULN, existingRecord.Uln);
            Assert.AreEqual(cohortDetails.Price, existingRecord.Cost);
            Assert.AreEqual(cohortDetails.TrainingCourse, existingRecord.CourseName);
            Assert.AreEqual(cohortDetails.FundingBandCapForExistingCohort, _trainingProgramme.ApprenticeshipFunding.FirstOrDefault().MaxEmployerLevyCap);
        }

        internal void VerifyFundingTextMappedCorrectlyForFileUploadedApprentices()
        {            
            Assert.AreEqual("2 apprenticeships above funding band maximum", _result.FundingBandTextForFileUploadCohorts);           
        }

        internal void VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentices()
        {
            Assert.AreEqual("The price for these apprenticeships is above the", _result.FundingBandInsetTextForFileUploadCohorts);
        }

        internal void VerifyFundingBandInsetTextMappedCorrectlyForFileUploadedApprentice()
        {
            Assert.AreEqual("The price for this apprenticeship is above its", _result.FundingBandInsetTextForFileUploadCohorts);
        }

        internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithDefaultData()
        {           
            // This will create three csv records, with total cost of 1000 (2 * 500)
            _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer", CohortRef, DateOfBirth, "2020-10-01", "2022-11", 500, 2));

            return this;
        }

        internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithPriceDefaultData(int numberOfApprentices)
        {
            _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer", CohortRef, DateOfBirth, "2020-10-01", "2022-11", 1500, numberOfApprentices));

            return this;
        }   

        private static IEnumerable<CsvRecord> CreateCsvRecords(IFixture fixture, string employerAgreementId, string cohortRef, string dateOfBirth, 
            string apprenticeStartDate, string apprenticeEndDate, int price,  int numberOfApprentices)
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
                .With(x => x.StartDate, _defaultStartDate)
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
               .With(x => x.StartDate, _defaultStartDate)
               .CreateMany(numberOfApprentices)
               .ToList()
            };
            _commitmentApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>()))
             .ReturnsAsync(_draftApprenticeshipsResponse);

            return this;
        }       

        internal void FundingTextMappedCorrectlyForExistingApprentices()
        {
            Assert.AreEqual("3 apprenticeships above funding band maximum", _result.FundingBandTextForExistingCohorts);
        }

        internal void FundingBandInsetTextMappedCorrectlyForExistingApprentices()
        {
            Assert.AreEqual("The price for these apprenticeships is above the", _result.FundingBandInsetTextForExistingCohorts);
        }

        internal void FundingBandInsetTextMappedCorrectlyForExistingApprentice()
        {
            Assert.AreEqual("The price for this apprenticeship is above its", _result.FundingBandInsetTextForExistingCohorts);
        }
    }
}

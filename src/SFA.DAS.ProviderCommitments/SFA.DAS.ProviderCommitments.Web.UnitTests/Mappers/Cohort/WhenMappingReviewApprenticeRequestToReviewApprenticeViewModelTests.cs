using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    public class WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTests
    {
        [Test]
        public async Task EmployerNameIsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();
            
            //Act
            await fixture.WithDefaultData().Action();

            //Assert
            fixture.VerifyEmployerNameIsMappedCorrectly();
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
        public async Task NumberOfApprenticesAreMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

            //Act
            await fixture.WithDefaultData().Action();

            //Assert
            fixture.VerifyNumberOfApprenticesAreMappedCorrectly();
        }

        [Test]
        public async Task TotalCostIsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

            //Act
            await fixture.WithDefaultData().Action();

            //Assert
            fixture.VerifyTotalCostIsMappedCorrectly();
        }

        [Test]
        public async Task CohortDetailsCountMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

            //Act
            await fixture.WithDefaultData().Action();

            //Assert
            fixture.VerifyCohortDetailsCountMappedCorrectly();
        }

        [Test]
        public async Task CohortDetailsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

            //Act
            await fixture.WithDefaultData().Action();

            //Assert
            fixture.VerifyCohortDetailsMappedCorrectly();
        }

        [Test]
        public async Task FundingTextMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();

            //Act
            await fixture.WithPriceDefaultData().Action();

            //Assert
            fixture.VerifyFundingText();
        }


        [Test]
        public async Task CohortRefTextMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture();
            

            //Act
            await fixture.WithOutCohortRefData().Action();
            

            //Assert
            fixture.VerifyCohortRefText();
        }

    }

    public class WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture
    {
        private Fixture fixture;
        private ReviewApprenticeRequestToReviewApprenticeViewModelMapper _sut;
        private FileUploadReviewApprenticeRequest _request;
        private Mock<IEncodingService> _encodingService;
        private Mock<ICommitmentsApiClient> _commitmentApiClient;
        private Mock<ICacheService> _cacheService;
        private List<CsvRecord> _csvRecords;
        private FileUploadReviewApprenticeViewModel _result;
        private List<TrainingProgrammeFundingPeriod> _fundingPeriods;
        private TrainingProgramme _trainingProgramme;
        private DateTime _startFundingPeriod = new DateTime(2020, 10, 1);
        private DateTime _endFundingPeriod = new DateTime(2020, 10, 30);
        public DateTime DefaultStartDate = new DateTime(2020, 10, 1);
        private const string cohortRef = "Cohort4";
        private const string dateOfBirth = "2001-09-05";

        public WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture()
        {
            fixture = new Fixture();
            _csvRecords = new List<CsvRecord>();

            _request = fixture.Create<FileUploadReviewApprenticeRequest>();
            _request.CohortRef = cohortRef;
            var accountLegalEntityEmployer = fixture.Build<AccountLegalEntityResponse>()
                .With(x => x.AccountName, "EmployerName").Create();

            var cohort = fixture.Build<GetCohortResponse>()
                .With(x => x.LatestMessageCreatedByEmployer, "A message").Create();

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode("Employer", EncodingType.PublicAccountLegalEntityId)).Returns(1);
            
            _commitmentApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(1, It.IsAny<CancellationToken>())).ReturnsAsync(accountLegalEntityEmployer);
            _commitmentApiClient.Setup(x => x.GetCohort(0, It.IsAny<CancellationToken>())).ReturnsAsync(cohort);

            _fundingPeriods = new List<TrainingProgrammeFundingPeriod>
            {
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, FundingCap = 1000},
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), FundingCap = 500}
            };
            _trainingProgramme = new TrainingProgramme { Name="CourseName", EffectiveFrom = DefaultStartDate, EffectiveTo = DefaultStartDate.AddYears(1), FundingPeriods = _fundingPeriods };

            _commitmentApiClient.Setup(x => x.GetTrainingProgramme(It.Is<string>(c => !string.IsNullOrEmpty(c)), CancellationToken.None))
                .ReturnsAsync(new GetTrainingProgrammeResponse { TrainingProgramme = _trainingProgramme });

            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.GetFromCache<List<CsvRecord>>(_request.CacheRequestId.ToString())).ReturnsAsync(_csvRecords);

            _sut = new ReviewApprenticeRequestToReviewApprenticeViewModelMapper(Mock.Of<ILogger<ReviewApprenticeRequestToReviewApprenticeViewModelMapper>>(), _commitmentApiClient.Object, _cacheService.Object, _encodingService.Object);
        }

        public async Task Action() => _result = await _sut.Map(_request);


        internal void VerifyEmployerNameIsMappedCorrectly()
        {            
            Assert.AreEqual("EmployerName", _result.EmployerName);
        }

        internal void VerifyCohortReferenceIsMappedCorrectly()
        {
            Assert.AreEqual(cohortRef, _result.CohortRef);
        }
        
        internal void VerifyNumberOfApprenticesAreMappedCorrectly()
        {
            var groupedByCohort = _csvRecords.Where(x => x.CohortRef == cohortRef);
            
            Assert.AreEqual(2, groupedByCohort.Count());
        }

        internal void VerifyTotalCostIsMappedCorrectly()
        {
            var groupedByCohort = _csvRecords.Where(x => x.CohortRef == cohortRef);

            Assert.AreEqual(1000, groupedByCohort.Sum(x => int.Parse(x.TotalPrice)));
        }

        internal void VerifyCohortDetailsCountMappedCorrectly()
        { 
            Assert.AreEqual(2, _result.CohortDetails.Count());
        }

        internal void VerifyCohortDetailsMappedCorrectly()
        {
            var csvRecord = _csvRecords.Where(x => x.CohortRef == cohortRef).FirstOrDefault();
            var cohortDetails = _result.CohortDetails[0];

            Assert.AreEqual(cohortDetails.Name, $"{csvRecord.GivenNames} {csvRecord.FamilyName}");
            Assert.AreEqual(cohortDetails.Email, csvRecord.EmailAddress);
            Assert.AreEqual(cohortDetails.ULN, csvRecord.ULN);
            Assert.AreEqual(dateOfBirth, csvRecord.DateOfBirth);
            Assert.AreEqual(cohortDetails.Price, int.Parse(csvRecord.TotalPrice));
            Assert.AreEqual(cohortDetails.TrainingCourse, _trainingProgramme.Name);
            Assert.AreEqual(cohortDetails.FundingBandCap, _trainingProgramme.FundingPeriods.FirstOrDefault().FundingCap);

        }
        internal void VerifyFundingText()
        {
            Assert.AreEqual("2 apprenticeships above funding band maximum", _result.FundingBandText);
            Assert.AreEqual(true, _result.CohortDetails.FirstOrDefault().ExceedsFundingBandCap);
            Assert.AreEqual(true, _result.CohortDetails.FirstOrDefault().FundingBandCap.HasValue);
            Assert.IsTrue(_result.CohortDetails.FirstOrDefault().Price > _result.CohortDetails.FirstOrDefault().FundingBandCap);
        }        

        internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithDefaultData()
        {           
            // This will create three csv records, with total cost of 1000 (2 * 500)
            _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer", cohortRef, dateOfBirth, "2020-10-01", "2022-11", 500, 2));

            return this;
        }

        internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithPriceDefaultData()
        {            
            // This will create three csv records, with total cost of 1000 (2 * 1500)
            _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer", cohortRef, dateOfBirth, "2020-10-01", "2022-11", 1500, 2));

            return this;
        }

        internal WhenMappingReviewApprenticeRequestToReviewApprenticeViewModelTestsFixture WithOutCohortRefData()
        {            
            _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer", string.Empty, dateOfBirth, "2020-10-01", "2022-11", 500, 1));

            return this;
        }

        internal void VerifyCohortRefText()
        {
            Assert.AreEqual("This will be created when you save or send to employers", _result.CohortRefText);          
        }

        private static List<CsvRecord> CreateCsvRecords(Fixture fixture, string employerAgreementId, string cohortRef, string dateOfBirth, 
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
    }
}

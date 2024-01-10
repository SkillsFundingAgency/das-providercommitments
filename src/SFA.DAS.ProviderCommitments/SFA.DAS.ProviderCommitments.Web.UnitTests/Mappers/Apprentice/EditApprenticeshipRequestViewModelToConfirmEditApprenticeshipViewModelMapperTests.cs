using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class EditApprenticeshipRequestViewModelToConfirmEditApprenticeshipViewModelMapperTests
    {
        ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture();
        }

        [Test]
        public async Task CommitmentApiToGetApprenticeshipIsCalled()
        {
            await fixture.Map();

            fixture.VerifyCommitmentApiIsCalled();
        }

        [Test]
        public async Task CommitmentApiToGetPriceEpisodeIsCalled()
        {
            await fixture.Map();

            fixture.VerifyPriceEpisodeIsCalled();
        }

        [Test]
        public async Task WhenOnlyProviderReferenceIsChanged()
        {
            fixture.source.ProviderReference = "ProviderRef";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.EmployerReference, Is.Not.EqualTo(fixture.source.ProviderReference));
            Assert.That(result.ProviderReference, Is.EqualTo(fixture.source.ProviderReference));
            Assert.That(result.OriginalApprenticeship.ProviderReference, Is.EqualTo(fixture._apprenticeshipResponse.ProviderReference));
        }

        [Test]
        public async Task WhenFirstNameIsChanged()
        {
            fixture.source.FirstName = "FirstName";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.FirstName, Is.Not.EqualTo(fixture.source.FirstName));
            Assert.That(result.FirstName, Is.EqualTo(fixture.source.FirstName));
            Assert.That(result.OriginalApprenticeship.FirstName, Is.EqualTo(fixture._apprenticeshipResponse.FirstName));
        }

        [Test]
        public async Task WhenLastNameIsChanged()
        {
            fixture.source.LastName = "LastName";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.LastName, Is.Not.EqualTo(fixture.source.LastName));
            Assert.That(result.LastName, Is.EqualTo(fixture.source.LastName));
            Assert.That(result.OriginalApprenticeship.LastName, Is.EqualTo(fixture._apprenticeshipResponse.LastName));
        }

        [Test]
        public async Task WhenEmailIsChanged()
        {
            fixture.source.Email = "Email";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.Email, Is.Not.EqualTo(fixture.source.Email));
            Assert.That(result.Email, Is.EqualTo(fixture.source.Email));
            Assert.That(result.OriginalApprenticeship.Email, Is.EqualTo(fixture._apprenticeshipResponse.Email));
        }

        [Test]
        public async Task WhenDobIsChanged()
        {
            fixture.source.DateOfBirth = new CommitmentsV2.Shared.Models.DateModel(new DateTime(2000, 12, 31));

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.DateOfBirth.Day, Is.Not.EqualTo(fixture.source.DateOfBirth.Day));
            Assert.That(result.DateOfBirth, Is.EqualTo(fixture.source.DateOfBirth.Date));
            Assert.That(result.OriginalApprenticeship.DateOfBirth, Is.EqualTo(fixture._apprenticeshipResponse.DateOfBirth));
        }

        [Test]
        public async Task WhenStartDateIsChanged()
        {
            var newStartDate = fixture._apprenticeshipResponse.StartDate.Value.AddMonths(-1);
            fixture.source.StartDate = new CommitmentsV2.Shared.Models.MonthYearModel(newStartDate.Month.ToString() + newStartDate.Year);

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.StartDate, Is.Not.EqualTo(fixture.source.StartDate.Date));
            Assert.That(result.StartDate, Is.EqualTo(fixture.source.StartDate.Date));
            Assert.That(result.OriginalApprenticeship.StartDate, Is.EqualTo(fixture._apprenticeshipResponse.StartDate));
        }

        [Test]
        public async Task WhenEndDateIsChanged()
        {
            var newEndDate = fixture._apprenticeshipResponse.EndDate.AddMonths(-1);
            fixture.source.EndDate = new CommitmentsV2.Shared.Models.MonthYearModel(newEndDate.Month.ToString() + newEndDate.Year);

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.EndDate, Is.Not.EqualTo(fixture.source.EndDate.Date));
            Assert.That(result.EndDate, Is.EqualTo(fixture.source.EndDate.Date));
            Assert.That(result.OriginalApprenticeship.EndDate, Is.EqualTo(fixture._apprenticeshipResponse.EndDate));
        }

        [Test]
        public async Task WhenCourseIsChanged()
        {
            fixture.source.CourseCode = "Abc";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.CourseCode, Is.Not.EqualTo(fixture.source.CourseCode));
            Assert.That(result.CourseCode, Is.EqualTo(fixture.source.CourseCode));
            Assert.That(result.OriginalApprenticeship.CourseCode, Is.EqualTo(fixture._apprenticeshipResponse.CourseCode));
        }

        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        public async Task WhenDeliveryModelIsChanged(DeliveryModel original, DeliveryModel changedTo)
        {
            fixture._apprenticeshipResponse.DeliveryModel = original;
            fixture.source.DeliveryModel = changedTo;

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.DeliveryModel, Is.Not.EqualTo(fixture.source.DeliveryModel));
            Assert.That(result.DeliveryModel, Is.EqualTo(fixture.source.DeliveryModel));
            Assert.That(result.OriginalApprenticeship.DeliveryModel, Is.EqualTo(fixture._apprenticeshipResponse.DeliveryModel));
        }

        [Test]
        public async Task WhenEmploymentEndDateIsChanged()
        {
            fixture.WithPortableFlexiJob();

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.EmploymentEndDate, Is.Not.EqualTo(fixture.source.EmploymentEndDate.Date));
            Assert.That(result.EmploymentEndDate, Is.EqualTo(fixture.source.EmploymentEndDate.Date));
            Assert.That(result.OriginalApprenticeship.EmploymentEndDate, Is.EqualTo(fixture._apprenticeshipResponse.EmploymentEndDate));
        }

        [Test]
        public async Task WhenEmploymentPriceIsChanged()
        {
            fixture.WithPortableFlexiJob();

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.EmploymentPrice, Is.Not.EqualTo(fixture.source.EmploymentPrice));
            Assert.That(result.EmploymentPrice, Is.EqualTo(fixture.source.EmploymentPrice));
            Assert.That(result.OriginalApprenticeship.EmploymentPrice, Is.EqualTo(fixture._apprenticeshipResponse.EmploymentPrice));
        }

        [Test]
        public async Task WhenVersionIsChanged()
        {
            fixture.source.Version = "1.1";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.Version, Is.Not.EqualTo(fixture.source.Version));
            Assert.That(result.Version, Is.EqualTo(fixture.source.Version));
        }

        [Test]
        public async Task WhenCourseCodeIsChangeButVersionIsNotChanged_ThenVersionIsMapped()
        {
            fixture.source.CourseCode = "123";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.Version, Is.Not.EqualTo(fixture.source.Version));
            Assert.That(result.Version, Is.EqualTo(fixture.source.Version));
        }

        [Test]
        public async Task WhenOptionIsChanged()
        {
            fixture.source.Option = "NewOption";

            var result = await fixture.Map();

            Assert.That(fixture._apprenticeshipResponse.Option, Is.Not.EqualTo(fixture.source.Option));
            Assert.That(result.Option, Is.EqualTo(fixture.source.Option));
        }

        [Test]
        public async Task When_VersionHasOptions_Then_ReturnToChangeOptionsIsTrue()
        {
            fixture.source.HasOptions = true;

            var result = await fixture.Map();

            Assert.That(result.ReturnToChangeOption, Is.True);
        }

        [Test]
        public async Task When_VersionIsChangedDirectly_Then_ReturnToChangeVersionIsTrue()
        {
            fixture.source.Version = "NewVersion";

            var result = await fixture.Map();

            Assert.That(result.ReturnToChangeVersion, Is.True);
        }

        [Test]
        public async Task When_VersionIsChangedByEditCourse_Then_ReturnToChangeVersionAndOptionAreFalse()
        {
            fixture.source.Version = "NewVersion";
            fixture.source.CourseCode = "NewCourseCode";

            var result = await fixture.Map();

            Assert.False(result.ReturnToChangeVersion);
            Assert.False(result.ReturnToChangeOption);
        }

        [Test]
        public async Task When_VersionIsChangedByEditStartDate_Then_ReturnToChangeVersionAndOptionAreFalse()
        {
            fixture.source.Version = "NewVersion";
            fixture.source.StartDate = new MonthYearModel(DateTime.Now.ToString("MMyyyy"));

            var result = await fixture.Map();

            Assert.False(result.ReturnToChangeVersion);
            Assert.False(result.ReturnToChangeOption);
        }

        [Test]
        public async Task WhenMultipleFieldsAreChanged_TheyAreChanged()
        {
            fixture.source.CourseCode = "NewCourse";
            fixture.source.LastName = "NewLastName";

            var result = await fixture.Map();

            Assert.That(result.LastName, Is.EqualTo(fixture.source.LastName));
            Assert.That(result.CourseCode, Is.EqualTo(fixture.source.CourseCode));
        }

        [Test]
        public async Task UnchangedFieldsAreNull()
        {
            fixture.source.CourseCode = "Course";

            var result = await fixture.Map();
            Assert.IsNull(result.FirstName);
            Assert.IsNull(result.LastName);
            Assert.IsNull(result.EndMonth);
            Assert.IsNull(result.StartMonth);
            Assert.IsNull(result.BirthMonth);
        }
    }

    public class ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;

        public GetApprenticeshipResponse _apprenticeshipResponse;
        private GetPriceEpisodesResponse _priceEpisodeResponse;

        private ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper _mapper;
        private TrainingProgramme _standardSummary;
        private Mock<IEncodingService> _encodingService;
        public EditApprenticeshipRequestViewModel source;
        public ConfirmEditApprenticeshipViewModel resultViewModl;

        public ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture()
        {
            var autoFixture = new Fixture();

            _apprenticeshipResponse = autoFixture.Build<GetApprenticeshipResponse>()
                .With(x => x.CourseCode, "ABC")
                .With(x => x.Version, "1.0")
                .With(x => x.StartDate, new DateTime(2020, 1, 1))
                .With(x => x.EndDate, new DateTime(2021, 1, 1))
                .With(x => x.DateOfBirth, new DateTime(1990, 1, 1))
                .Without(x=>x.EmploymentEndDate)
                .Without(x=>x.EmploymentPrice)
                .Create();

            source = new EditApprenticeshipRequestViewModel();
            source.ApprenticeshipId = _apprenticeshipResponse.Id;
            source.CourseCode = _apprenticeshipResponse.CourseCode;
            source.FirstName = _apprenticeshipResponse.FirstName;
            source.LastName = _apprenticeshipResponse.LastName;
            source.Email = _apprenticeshipResponse.Email;
            source.DateOfBirth = new DateModel(_apprenticeshipResponse.DateOfBirth);
            source.Cost = 1000;
            source.ProviderReference = _apprenticeshipResponse.ProviderReference;
            source.StartDate = new MonthYearModel(_apprenticeshipResponse.StartDate.Value.Month.ToString() + _apprenticeshipResponse.StartDate.Value.Year);
            source.EndDate = new MonthYearModel(_apprenticeshipResponse.EndDate.Month.ToString() + _apprenticeshipResponse.EndDate.Year);

            _priceEpisodeResponse = autoFixture.Build<GetPriceEpisodesResponse>()
                .With(x => x.PriceEpisodes, new List<PriceEpisode> {
                    new() { Cost = 1000, FromDate = DateTime.Now.AddMonths(-1), ToDate = null}})
                .Create();

            _standardSummary = autoFixture.Create<TrainingProgramme>();
            _standardSummary.EffectiveFrom = new DateTime(2018, 1, 1);
            _standardSummary.EffectiveTo = new DateTime(2022, 1, 1);
            _standardSummary.FundingPeriods = SetPriceBand(1000);

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apprenticeshipResponse);
            _mockCommitmentsApiClient.Setup(c => c.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_priceEpisodeResponse);
            _mockCommitmentsApiClient.Setup(t => t.GetTrainingProgrammeVersionByCourseCodeAndVersion(source.CourseCode, source.Version, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTrainingProgrammeResponse
                {
                    TrainingProgramme = _standardSummary
                });

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.ApprenticeshipId)).Returns(_apprenticeshipResponse.Id);
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.AccountId)).Returns(_apprenticeshipResponse.EmployerAccountId);

            _mapper = new ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapper(_mockCommitmentsApiClient.Object, _encodingService.Object);
        }

        public ConfirmEditApprenticeshipRequestToConfirmEditViewModelMapperTestsFixture WithPortableFlexiJob()
        {
            _apprenticeshipResponse.DeliveryModel = DeliveryModel.PortableFlexiJob;
            _apprenticeshipResponse.EmploymentPrice = 500;
            _apprenticeshipResponse.EmploymentEndDate = new DateTime(2020,09,01);
            return this;
        } 

        public List<TrainingProgrammeFundingPeriod> SetPriceBand(int fundingCap)
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
            resultViewModl = await _mapper.Map(source);
            return resultViewModl;
        }

        internal void VerifyCommitmentApiIsCalled()
        {
            _mockCommitmentsApiClient.Verify(c => c.GetApprenticeship(_apprenticeshipResponse.Id, It.IsAny<CancellationToken>()), Times.Once());
        }

        internal void VerifyPriceEpisodeIsCalled()
        {
            _mockCommitmentsApiClient.Verify(c => c.GetPriceEpisodes(_apprenticeshipResponse.Id, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}

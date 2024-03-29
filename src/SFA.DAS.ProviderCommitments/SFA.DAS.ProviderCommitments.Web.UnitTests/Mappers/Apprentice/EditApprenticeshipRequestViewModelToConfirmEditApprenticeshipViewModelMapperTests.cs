﻿using SFA.DAS.CommitmentsV2.Api.Client;
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

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.EmployerReference, Is.Not.EqualTo(fixture.source.ProviderReference));
                Assert.That(result.ProviderReference, Is.EqualTo(fixture.source.ProviderReference));
                Assert.That(result.OriginalApprenticeship.ProviderReference, Is.EqualTo(fixture.ApprenticeshipResponse.ProviderReference));
            });
        }

        [Test]
        public async Task WhenFirstNameIsChanged()
        {
            fixture.source.FirstName = "FirstName";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.FirstName, Is.Not.EqualTo(fixture.source.FirstName));
                Assert.That(result.FirstName, Is.EqualTo(fixture.source.FirstName));
                Assert.That(result.OriginalApprenticeship.FirstName, Is.EqualTo(fixture.ApprenticeshipResponse.FirstName));
            });
        }

        [Test]
        public async Task WhenLastNameIsChanged()
        {
            fixture.source.LastName = "LastName";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.LastName, Is.Not.EqualTo(fixture.source.LastName));
                Assert.That(result.LastName, Is.EqualTo(fixture.source.LastName));
                Assert.That(result.OriginalApprenticeship.LastName, Is.EqualTo(fixture.ApprenticeshipResponse.LastName));
            });
        }

        [Test]
        public async Task WhenEmailIsChanged()
        {
            fixture.source.Email = "Email";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.Email, Is.Not.EqualTo(fixture.source.Email));
                Assert.That(result.Email, Is.EqualTo(fixture.source.Email));
                Assert.That(result.OriginalApprenticeship.Email, Is.EqualTo(fixture.ApprenticeshipResponse.Email));
            });
        }

        [Test]
        public async Task WhenDobIsChanged()
        {
            fixture.source.DateOfBirth = new DateModel(new DateTime(2000, 12, 31));

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.DateOfBirth.Day, Is.Not.EqualTo(fixture.source.DateOfBirth.Day));
                Assert.That(result.DateOfBirth, Is.EqualTo(fixture.source.DateOfBirth.Date));
                Assert.That(result.OriginalApprenticeship.DateOfBirth, Is.EqualTo(fixture.ApprenticeshipResponse.DateOfBirth));
            });
        }

        [Test]
        public async Task WhenStartDateIsChanged()
        {
            var newStartDate = fixture.ApprenticeshipResponse.StartDate.Value.AddMonths(-1);
            fixture.source.StartDate = new MonthYearModel(newStartDate.Month.ToString() + newStartDate.Year);

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.StartDate, Is.Not.EqualTo(fixture.source.StartDate.Date));
                Assert.That(result.StartDate, Is.EqualTo(fixture.source.StartDate.Date));
                Assert.That(result.OriginalApprenticeship.StartDate, Is.EqualTo(fixture.ApprenticeshipResponse.StartDate));
            });
        }

        [Test]
        public async Task WhenEndDateIsChanged()
        {
            var newEndDate = fixture.ApprenticeshipResponse.EndDate.AddMonths(-1);
            fixture.source.EndDate = new MonthYearModel(newEndDate.Month.ToString() + newEndDate.Year);

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.EndDate, Is.Not.EqualTo(fixture.source.EndDate.Date));
                Assert.That(result.EndDate, Is.EqualTo(fixture.source.EndDate.Date));
                Assert.That(result.OriginalApprenticeship.EndDate, Is.EqualTo(fixture.ApprenticeshipResponse.EndDate));
            });
        }

        [Test]
        public async Task WhenCourseIsChanged()
        {
            fixture.source.CourseCode = "Abc";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.CourseCode, Is.Not.EqualTo(fixture.source.CourseCode));
                Assert.That(result.CourseCode, Is.EqualTo(fixture.source.CourseCode));
                Assert.That(result.OriginalApprenticeship.CourseCode, Is.EqualTo(fixture.ApprenticeshipResponse.CourseCode));
            });
        }

        [TestCase(DeliveryModel.Regular, DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.PortableFlexiJob, DeliveryModel.Regular)]
        public async Task WhenDeliveryModelIsChanged(DeliveryModel original, DeliveryModel changedTo)
        {
            fixture.ApprenticeshipResponse.DeliveryModel = original;
            fixture.source.DeliveryModel = changedTo;

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.DeliveryModel, Is.Not.EqualTo(fixture.source.DeliveryModel));
                Assert.That(result.DeliveryModel, Is.EqualTo(fixture.source.DeliveryModel));
                Assert.That(result.OriginalApprenticeship.DeliveryModel, Is.EqualTo(fixture.ApprenticeshipResponse.DeliveryModel));
            });
        }

        [Test]
        public async Task WhenEmploymentEndDateIsChanged()
        {
            fixture.WithPortableFlexiJob();

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.EmploymentEndDate, Is.Not.EqualTo(fixture.source.EmploymentEndDate.Date));
                Assert.That(result.EmploymentEndDate, Is.EqualTo(fixture.source.EmploymentEndDate.Date));
                Assert.That(result.OriginalApprenticeship.EmploymentEndDate, Is.EqualTo(fixture.ApprenticeshipResponse.EmploymentEndDate));
            });
        }

        [Test]
        public async Task WhenEmploymentPriceIsChanged()
        {
            fixture.WithPortableFlexiJob();

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.EmploymentPrice, Is.Not.EqualTo(fixture.source.EmploymentPrice));
                Assert.That(result.EmploymentPrice, Is.EqualTo(fixture.source.EmploymentPrice));
                Assert.That(result.OriginalApprenticeship.EmploymentPrice, Is.EqualTo(fixture.ApprenticeshipResponse.EmploymentPrice));
            });
        }

        [Test]
        public async Task WhenVersionIsChanged()
        {
            fixture.source.Version = "1.1";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.Version, Is.Not.EqualTo(fixture.source.Version));
                Assert.That(result.Version, Is.EqualTo(fixture.source.Version));
            });
        }

        [Test]
        public async Task WhenCourseCodeIsChangeButVersionIsNotChanged_ThenVersionIsMapped()
        {
            fixture.source.CourseCode = "123";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.Version, Is.Not.EqualTo(fixture.source.Version));
                Assert.That(result.Version, Is.EqualTo(fixture.source.Version));
            });
        }

        [Test]
        public async Task WhenOptionIsChanged()
        {
            fixture.source.Option = "NewOption";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.ApprenticeshipResponse.Option, Is.Not.EqualTo(fixture.source.Option));
                Assert.That(result.Option, Is.EqualTo(fixture.source.Option));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(result.ReturnToChangeVersion, Is.False);
                Assert.That(result.ReturnToChangeOption, Is.False);
            });
        }

        [Test]
        public async Task When_VersionIsChangedByEditStartDate_Then_ReturnToChangeVersionAndOptionAreFalse()
        {
            fixture.source.Version = "NewVersion";
            fixture.source.StartDate = new MonthYearModel(DateTime.Now.ToString("MMyyyy"));

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(result.ReturnToChangeVersion, Is.False);
                Assert.That(result.ReturnToChangeOption, Is.False);
            });
        }

        [Test]
        public async Task WhenMultipleFieldsAreChanged_TheyAreChanged()
        {
            fixture.source.CourseCode = "NewCourse";
            fixture.source.LastName = "NewLastName";

            var result = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(result.LastName, Is.EqualTo(fixture.source.LastName));
                Assert.That(result.CourseCode, Is.EqualTo(fixture.source.CourseCode));
            });
        }

        [Test]
        public async Task UnchangedFieldsAreNull()
        {
            fixture.source.CourseCode = "Course";

            var result = await fixture.Map();
            Assert.Multiple(() =>
            {
                Assert.That(result.FirstName, Is.Null);
                Assert.That(result.LastName, Is.Null);
                Assert.That(result.EndMonth, Is.Null);
                Assert.That(result.StartMonth, Is.Null);
                Assert.That(result.BirthMonth, Is.Null);
            });
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
                .Without(x=>x.EmploymentEndDate)
                .Without(x=>x.EmploymentPrice)
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
                .With(x => x.PriceEpisodes, new List<PriceEpisode> {
                    new() { Cost = 1000, FromDate = DateTime.Now.AddMonths(-1), ToDate = null}})
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
            ApprenticeshipResponse.EmploymentEndDate = new DateTime(2020,09,01);
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
}

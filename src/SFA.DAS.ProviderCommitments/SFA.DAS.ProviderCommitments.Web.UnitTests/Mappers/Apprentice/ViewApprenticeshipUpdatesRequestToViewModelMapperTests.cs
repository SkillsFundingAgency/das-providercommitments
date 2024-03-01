using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetApprenticeshipUpdatesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ViewApprenticeshipUpdatesRequestToViewModelMapperTests
    {
        ViewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new ViewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture();
        }

        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipHashedId, Is.EqualTo(_fixture.Source.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ProviderId, Is.EqualTo(_fixture.Source.ProviderId));
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(_fixture.ApprenticeshipUpdate.FirstName));
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(_fixture.ApprenticeshipUpdate.LastName));
        }

        [Test]
        public async Task Email_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Email, Is.EqualTo(_fixture.ApprenticeshipUpdate.Email));
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DateOfBirth, Is.EqualTo(_fixture.ApprenticeshipUpdate.DateOfBirth));
        }

        [Test]
        public async Task Cost_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Cost, Is.EqualTo(_fixture.ApprenticeshipUpdate.Cost));
        }

        [Test]
        public async Task StartDate_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.StartDate, Is.EqualTo(_fixture.ApprenticeshipUpdate.StartDate));
        }

        [Test]
        public async Task EndDate_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EndDate, Is.EqualTo(_fixture.ApprenticeshipUpdate.EndDate));
        }

        [Test]
        public async Task DeliveryModel_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DeliveryModel, Is.EqualTo(_fixture.ApprenticeshipUpdate.DeliveryModel));
        }

        [Test]
        public async Task EmploymentEndDate_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentEndDate, Is.EqualTo(_fixture.ApprenticeshipUpdate.EmploymentEndDate));
        }

        [Test]
        public async Task EmploymentPrice_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentPrice, Is.EqualTo(_fixture.ApprenticeshipUpdate.EmploymentPrice));
        }

        [Test]
        public async Task CourseCode_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseCode, Is.EqualTo(_fixture.ApprenticeshipUpdate.TrainingCode));
        }

        [Test]
        public async Task Version_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Version, Is.EqualTo(_fixture.ApprenticeshipUpdate.Version));
        }

        [Test]
        public async Task Option_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Option, Is.EqualTo(_fixture.ApprenticeshipUpdate.Option));
        }

        [Test]
        public async Task TrainingName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseName, Is.EqualTo(_fixture.ApprenticeshipUpdate.TrainingName));
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ProviderName, Is.EqualTo(_fixture.GetApprenticeshipResponse.ProviderName));
        }

        [Test]
        public async Task OriginalApprenticeship_FirstName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(_fixture.ApprenticeshipUpdate.FirstName));
        }

        [Test]
        public async Task OriginalApprenticeship_LastName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(_fixture.ApprenticeshipUpdate.LastName));
        }

        [Test]
        public async Task OriginalApprenticeship_DateOfBirth_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DateOfBirth, Is.EqualTo(_fixture.ApprenticeshipUpdate.DateOfBirth));
        }

        [Test]
        public async Task OriginalApprenticeship_Cost_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Cost, Is.EqualTo(_fixture.ApprenticeshipUpdate.Cost));
        }

        [Test]
        public async Task OriginalApprenticeship_StartDate_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.StartDate, Is.EqualTo(_fixture.ApprenticeshipUpdate.StartDate));
        }

        [Test]
        public async Task OriginalApprenticeship_EndDate_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EndDate, Is.EqualTo(_fixture.ApprenticeshipUpdate.EndDate));
        }

        [Test]
        public async Task OriginalApprenticeship_CourseCode_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseCode, Is.EqualTo(_fixture.ApprenticeshipUpdate.TrainingCode));
        }

        [Test]
        public async Task OriginalApprenticeship_Version_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Version, Is.EqualTo(_fixture.ApprenticeshipUpdate.Version));
        }

        [Test]
        public async Task OriginalApprenticeship_Option_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Option, Is.EqualTo(_fixture.ApprenticeshipUpdate.Option));
        }

        [Test]
        public async Task OriginalApprenticeship_TrainingName_IsMapped()
        {
            var viewModel = await _fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseName, Is.EqualTo(_fixture.ApprenticeshipUpdate.TrainingName));
        }

        private class ViewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture
        {
            private readonly ViewApprenticeshipUpdatesRequestToViewModelMapper _mapper;
            private const long ApprenticeshipId = 1;

            public GetApprenticeshipResponse GetApprenticeshipResponse { get; }
            public GetApprenticeshipUpdatesRequest GetApprenticeshipUpdatesRequest { get; }
            public ApprenticeshipUpdate ApprenticeshipUpdate { get; }
            public ViewApprenticeshipUpdatesRequest Source { get; }

            public ViewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture()
            {
                var autoFixture = new Fixture();
                autoFixture.Customizations.Add(new DateTimeSpecimenBuilder());
                var commitmentApiClient = new Mock<ICommitmentsApiClient>();

                Source = new ViewApprenticeshipUpdatesRequest
                    { ApprenticeshipId = ApprenticeshipId, ProviderId = 22, ApprenticeshipHashedId = "XXX" };
                GetApprenticeshipResponse = autoFixture.Create<GetApprenticeshipResponse>();
                GetApprenticeshipResponse.Id = ApprenticeshipId;
                autoFixture.RepeatCount = 1;
                var getApprenticeshipUpdatesResponses = autoFixture.Create<GetApprenticeshipUpdatesResponse>();
                ApprenticeshipUpdate = getApprenticeshipUpdatesResponses.ApprenticeshipUpdates.First();
                var getTrainingProgrammeResponse = autoFixture.Create<GetTrainingProgrammeResponse>();

                var priceEpisode = new GetPriceEpisodesResponse
                {
                    PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>()
                    {
                        new()
                        {
                            FromDate = DateTime.UtcNow.AddDays(-10),
                            ToDate = null,
                            Cost = 100
                        }
                    }
                };

                commitmentApiClient.Setup(x => x.GetApprenticeship(ApprenticeshipId, It.IsAny<CancellationToken>()))
                    .Returns(() => Task.FromResult(GetApprenticeshipResponse));
                commitmentApiClient
                    .Setup(x => x.GetApprenticeshipUpdates(ApprenticeshipId,
                        It.IsAny<GetApprenticeshipUpdatesRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(() => Task.FromResult(getApprenticeshipUpdatesResponses));
                commitmentApiClient.Setup(x => x.GetPriceEpisodes(ApprenticeshipId, It.IsAny<CancellationToken>()))
                    .Returns(() => Task.FromResult(priceEpisode));
                commitmentApiClient
                    .Setup(x => x.GetTrainingProgramme(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns(() => Task.FromResult(getTrainingProgrammeResponse));

                _mapper = new ViewApprenticeshipUpdatesRequestToViewModelMapper(commitmentApiClient.Object);
            }

            internal async Task<ViewApprenticeshipUpdatesViewModel> Map()
            {
                return await _mapper.Map(Source);
            }
        }

        private class DateTimeSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var pi = request as PropertyInfo;
                if (pi == null || pi.PropertyType != typeof(DateTime?))
                {
                    return new NoSpecimen();
                }

                var randomDateTime = context.Create<DateTime>();

                var dateTime = pi.Name == "DateOfBirth" 
                    ? new DateTime(randomDateTime.Year, randomDateTime.Month, randomDateTime.Day) 
                    : new DateTime(randomDateTime.Year, randomDateTime.Month, 1);

                return dateTime;
            }
        }
    }
}
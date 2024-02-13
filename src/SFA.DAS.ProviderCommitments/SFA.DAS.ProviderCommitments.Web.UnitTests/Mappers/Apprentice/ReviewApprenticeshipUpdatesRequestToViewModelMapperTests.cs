using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture.Kernel;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ReviewApprenticeshipUpdatesRequestToViewModelMapperTests
    {
        ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture fixture;
        [SetUp]
        public void Arrange()
        {
            fixture = new ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture();
        }

        [Test]
        public async Task IsValidCourseCode_IsMapped()
        {

            fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.CourseCode = "ABC";
            var viewModel = await fixture.Map();

            Assert.That(viewModel.IsValidCourseCode, Is.EqualTo(fixture.GetReviewApprenticeshipUpdatesResponse.IsValidCourseCode));
        }

        [Test]
        public async Task ApprenticeshipHashedId_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipHashedId, Is.EqualTo(fixture.Source.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ProviderId_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ProviderId, Is.EqualTo(fixture.Source.ProviderId));
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(fixture.ApprenticeshipUpdate.FirstName));
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(fixture.ApprenticeshipUpdate.LastName));
        }

        [Test]
        public async Task DateOfBirth_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DateOfBirth, Is.EqualTo(fixture.ApprenticeshipUpdate.DateOfBirth));
        }

        [Test]
        public async Task Cost_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Cost, Is.EqualTo(fixture.ApprenticeshipUpdate.Cost));
        }

        [Test]
        public async Task StartDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.StartDate, Is.EqualTo(fixture.ApprenticeshipUpdate.StartDate));
        }

        [Test]
        public async Task EndDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EndDate, Is.EqualTo(fixture.ApprenticeshipUpdate.EndDate));
        }

        [Test]
        public async Task DeliveryModel_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DeliveryModel, Is.EqualTo(fixture.ApprenticeshipUpdate.DeliveryModel));
        }

        [Test]
        public async Task EmploymentEndDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentEndDate, Is.EqualTo(fixture.ApprenticeshipUpdate.EmploymentEndDate));
        }

        [Test]
        public async Task EmploymentPrice_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentPrice, Is.EqualTo(fixture.ApprenticeshipUpdate.EmploymentPrice));
        }

        [Test]
        public async Task CourseCode_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseCode, Is.EqualTo(fixture.ApprenticeshipUpdate.CourseCode));
        }

        [Test]
        public async Task Email_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Email, Is.EqualTo(fixture.ApprenticeshipUpdate.Email));
        }

        [Test]
        public async Task TrainingName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseName, Is.EqualTo(fixture.ApprenticeshipUpdate.CourseName));
        }

        [Test]
        public async Task Version_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Version, Is.EqualTo(fixture.ApprenticeshipUpdate.Version));
        }

        [Test]
        public async Task Option_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Option, Is.EqualTo(fixture.ApprenticeshipUpdate.Option));
        }

        [Test]
        public async Task ProviderName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ProviderName, Is.EqualTo(fixture.GetReviewApprenticeshipUpdatesResponse.ProviderName));
        }

        [Test]
        public async Task OriginalApprenticeship_FirstName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(fixture.ApprenticeshipUpdate.FirstName));
        }

        [Test]
        public async Task OriginalApprenticeship_LastName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(fixture.ApprenticeshipUpdate.LastName));
        }

        [Test]
        public async Task OriginalApprenticeship_DateOfBirth_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DateOfBirth, Is.EqualTo(fixture.ApprenticeshipUpdate.DateOfBirth));
        }

        [Test]
        public async Task OriginalApprenticeship_Cost_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Cost, Is.EqualTo(fixture.ApprenticeshipUpdate.Cost));
        }

        [Test] public async Task OriginalApprenticeship_DeliveryModel_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.DeliveryModel, Is.EqualTo(fixture.ApprenticeshipUpdate.DeliveryModel));
        }

        [Test]
        public async Task OriginalApprenticeship_EmploymentEndDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentEndDate, Is.EqualTo(fixture.ApprenticeshipUpdate.EmploymentEndDate));
        }

        [Test]
        public async Task OriginalApprenticeship_EmploymentPrice_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EmploymentPrice, Is.EqualTo(fixture.ApprenticeshipUpdate.EmploymentPrice));
        }

        [Test]
        public async Task OriginalApprenticeship_StartDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.StartDate, Is.EqualTo(fixture.ApprenticeshipUpdate.StartDate));
        }

        [Test]
        public async Task OriginalApprenticeship_EndDate_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.EndDate, Is.EqualTo(fixture.ApprenticeshipUpdate.EndDate));
        }

        [Test]
        public async Task OriginalApprenticeship_CourseCode_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseCode, Is.EqualTo(fixture.ApprenticeshipUpdate.CourseCode));
        }

        [Test]
        public async Task OriginalApprenticeship_TrainingName_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.CourseName, Is.EqualTo(fixture.ApprenticeshipUpdate.CourseName));
        }

        [Test]
        public async Task OriginalApprenticeship_Version_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Version, Is.EqualTo(fixture.ApprenticeshipUpdate.Version));
        }

        [Test]
        public async Task OriginalApprenticeship_Option_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Option, Is.EqualTo(fixture.ApprenticeshipUpdate.Option));
        }

        [Test]
        public async Task If_FirstName_Only_Updated_Map_FirstName_From_OriginalApprenticeship()
        {
            fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.LastName = null;

            var viewModel = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(fixture.ApprenticeshipUpdate.FirstName));
                Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(fixture.GetReviewApprenticeshipUpdatesResponse.OriginalApprenticeship.LastName));
            });
        }

        [Test]
        public async Task If_LastName_Only_Updated_Map_FirstName_From_OriginalApprenticeship()
        {
            fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.FirstName = null;

            var viewModel = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(fixture.ApprenticeshipUpdate.LastName));
                Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(fixture.GetReviewApprenticeshipUpdatesResponse.OriginalApprenticeship.FirstName));
            });
        }

        [Test]
        public async Task If_BothNames_Updated_Map_BothNames_From_Update()
        {
            var viewModel = await fixture.Map();

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ApprenticeshipUpdates.FirstName, Is.EqualTo(fixture.ApprenticeshipUpdate.FirstName));
                Assert.That(viewModel.ApprenticeshipUpdates.LastName, Is.EqualTo(fixture.ApprenticeshipUpdate.LastName));
            });
        }

        [Test]
        public async Task OriginalApprenticeship_Email_IsMapped()
        {
            var viewModel = await fixture.Map();

            Assert.That(viewModel.ApprenticeshipUpdates.Email, Is.EqualTo(fixture.ApprenticeshipUpdate.Email));
        }

        public class ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture
        {
            public ReviewApprenticeshipUpdatesRequestToViewModelMapper Mapper;
            public ReviewApprenticeshipUpdatesRequest Source;
            public Mock<ICommitmentsApiClient> CommitmentApiClient;
            public Mock<IOuterApiClient> ApiClient;
            public GetApprenticeshipUpdatesRequest GetApprenticeshipUpdatesRequest;
            public GetTrainingProgrammeResponse GetTrainingProgrammeResponse;
            public ApprenticeshipDetails ApprenticeshipUpdate;

            public GetReviewApprenticeshipUpdatesResponse GetReviewApprenticeshipUpdatesResponse;

            public long ApprenticeshipId = 1;

            public ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture()
            {
                var autoFixture = new Fixture();
                autoFixture.Customizations.Add(new DateTimeSpecimenBuilder());
                CommitmentApiClient = new Mock<ICommitmentsApiClient>();
                ApiClient = new Mock<IOuterApiClient>();

                Source = new ReviewApprenticeshipUpdatesRequest { ApprenticeshipId = ApprenticeshipId, ProviderId = 22, ApprenticeshipHashedId = "XXX" };

                autoFixture.RepeatCount = 1;
              
                GetTrainingProgrammeResponse = autoFixture.Create<GetTrainingProgrammeResponse>();
                GetReviewApprenticeshipUpdatesResponse = autoFixture.Create<GetReviewApprenticeshipUpdatesResponse>();
                ApprenticeshipUpdate = GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates;

                var priceEpisode = new GetPriceEpisodesResponse
                {
                    PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>(){ new()
                    {
                    FromDate = DateTime.UtcNow.AddDays(-10),
                    ToDate = null,
                    Cost = 100
                    } }
                };

                CommitmentApiClient.Setup(x => x.GetPriceEpisodes(ApprenticeshipId, It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(priceEpisode));
                CommitmentApiClient.Setup(x => x.GetTrainingProgramme(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(GetTrainingProgrammeResponse));

                ApiClient.Setup(x =>
                        x.Get<GetReviewApprenticeshipUpdatesResponse>(
                            It.Is<GetReviewApprenticeshipUpdatesRequest>(r =>
                                r.ApprenticeshipId == ApprenticeshipId)))
                    .ReturnsAsync(GetReviewApprenticeshipUpdatesResponse);

                Mapper = new ReviewApprenticeshipUpdatesRequestToViewModelMapper(CommitmentApiClient.Object, ApiClient.Object);
            }

            internal async Task<ReviewApprenticeshipUpdatesViewModel> Map()
            {
                return await Mapper.Map(Source);
            }
        }

        public class DateTimeSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var pi = request as PropertyInfo;
                if (pi == null || pi.PropertyType != typeof(DateTime?))
                    return new NoSpecimen();

                else
                {
                    DateTime dt;
                    var randomDateTime = context.Create<DateTime>();

                    if (pi.Name == "DateOfBirth")
                    {
                        dt = new DateTime(randomDateTime.Year, randomDateTime.Month, randomDateTime.Day);
                    }
                    else
                    {
                        dt = new DateTime(randomDateTime.Year, randomDateTime.Month, 1);
                    }

                    return dt;
                }
            }
        }
    }
}

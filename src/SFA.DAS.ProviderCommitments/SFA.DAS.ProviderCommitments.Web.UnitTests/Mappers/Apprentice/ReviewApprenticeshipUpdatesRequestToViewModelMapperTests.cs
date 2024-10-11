using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture.Kernel;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class ReviewApprenticeshipUpdatesRequestToViewModelMapperTests
{
    private ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture _fixture;
    
    [SetUp]
    public void Arrange() => _fixture = new ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture();

    [Test]
    public async Task IsValidCourseCode_IsMapped()
    {
        _fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.CourseCode = "ABC";
        var viewModel = await _fixture.Map();

        viewModel.IsValidCourseCode.Should().Be(_fixture.GetReviewApprenticeshipUpdatesResponse.IsValidCourseCode);
    }

    [Test]
    public async Task ApprenticeshipHashedId_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipHashedId.Should().Be(_fixture.Source.ApprenticeshipHashedId);
    }

    [Test]
    public async Task ProviderId_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ProviderId.Should().Be(_fixture.Source.ProviderId);
    }

    [Test]
    public async Task FirstName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.FirstName.Should().Be(_fixture.ApprenticeshipUpdate.FirstName);
    }

    [Test]
    public async Task LastName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.LastName.Should().Be(_fixture.ApprenticeshipUpdate.LastName);
    }

    [Test]
    public async Task DateOfBirth_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.DateOfBirth.Should().Be(_fixture.ApprenticeshipUpdate.DateOfBirth);
    }

    [Test]
    public async Task Cost_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Cost.Should().Be(_fixture.ApprenticeshipUpdate.Cost);
    }

    [Test]
    public async Task StartDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.StartDate.Should().Be(_fixture.ApprenticeshipUpdate.StartDate);
    }

    [Test]
    public async Task EndDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EndDate.Should().Be(_fixture.ApprenticeshipUpdate.EndDate);
    }

    [Test]
    public async Task DeliveryModel_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.DeliveryModel.Should().Be(_fixture.ApprenticeshipUpdate.DeliveryModel);
    }

    [Test]
    public async Task EmploymentEndDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EmploymentEndDate.Should().Be(_fixture.ApprenticeshipUpdate.EmploymentEndDate);
    }

    [Test]
    public async Task EmploymentPrice_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EmploymentPrice.Should().Be(_fixture.ApprenticeshipUpdate.EmploymentPrice);
    }

    [Test]
    public async Task CourseCode_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.CourseCode.Should().Be(_fixture.ApprenticeshipUpdate.CourseCode);
    }

    [Test]
    public async Task Email_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Email.Should().Be(_fixture.ApprenticeshipUpdate.Email);
    }

    [Test]
    public async Task TrainingName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.CourseName.Should().Be(_fixture.ApprenticeshipUpdate.CourseName);
    }

    [Test]
    public async Task Version_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Version.Should().Be(_fixture.ApprenticeshipUpdate.Version);
    }

    [Test]
    public async Task Option_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Option.Should().Be(_fixture.ApprenticeshipUpdate.Option);
    }

    [Test]
    public async Task ProviderName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ProviderName.Should().Be(_fixture.GetReviewApprenticeshipUpdatesResponse.ProviderName);
    }

    [Test]
    public async Task OriginalApprenticeship_FirstName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.FirstName.Should().Be(_fixture.ApprenticeshipUpdate.FirstName);
    }

    [Test]
    public async Task OriginalApprenticeship_LastName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.LastName.Should().Be(_fixture.ApprenticeshipUpdate.LastName);
    }

    [Test]
    public async Task OriginalApprenticeship_DateOfBirth_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.DateOfBirth.Should().Be(_fixture.ApprenticeshipUpdate.DateOfBirth);
    }

    [Test]
    public async Task OriginalApprenticeship_Cost_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Cost.Should().Be(_fixture.ApprenticeshipUpdate.Cost);
    }

    [Test] public async Task OriginalApprenticeship_DeliveryModel_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.DeliveryModel.Should().Be(_fixture.ApprenticeshipUpdate.DeliveryModel);
    }

    [Test]
    public async Task OriginalApprenticeship_EmploymentEndDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EmploymentEndDate.Should().Be(_fixture.ApprenticeshipUpdate.EmploymentEndDate);
    }

    [Test]
    public async Task OriginalApprenticeship_EmploymentPrice_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EmploymentPrice.Should().Be(_fixture.ApprenticeshipUpdate.EmploymentPrice);
    }

    [Test]
    public async Task OriginalApprenticeship_StartDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.StartDate.Should().Be(_fixture.ApprenticeshipUpdate.StartDate);
    }

    [Test]
    public async Task OriginalApprenticeship_EndDate_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.EndDate.Should().Be(_fixture.ApprenticeshipUpdate.EndDate);
    }

    [Test]
    public async Task OriginalApprenticeship_CourseCode_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.CourseCode.Should().Be(_fixture.ApprenticeshipUpdate.CourseCode);
    }

    [Test]
    public async Task OriginalApprenticeship_TrainingName_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.CourseName.Should().Be(_fixture.ApprenticeshipUpdate.CourseName);
    }

    [Test]
    public async Task OriginalApprenticeship_Version_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Version.Should().Be(_fixture.ApprenticeshipUpdate.Version);
    }

    [Test]
    public async Task OriginalApprenticeship_Option_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Option.Should().Be(_fixture.ApprenticeshipUpdate.Option);
    }

    [Test]
    public async Task If_FirstName_Only_Updated_Map_FirstName_From_OriginalApprenticeship()
    {
        _fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.LastName = null;

        var viewModel = await _fixture.Map();

        using (new AssertionScope())
        {
            viewModel.ApprenticeshipUpdates.FirstName.Should().Be(_fixture.ApprenticeshipUpdate.FirstName);
            viewModel.ApprenticeshipUpdates.LastName.Should().Be(_fixture.GetReviewApprenticeshipUpdatesResponse.OriginalApprenticeship.LastName);
        }
    }

    [Test]
    public async Task If_LastName_Only_Updated_Map_FirstName_From_OriginalApprenticeship()
    {
        _fixture.GetReviewApprenticeshipUpdatesResponse.ApprenticeshipUpdates.FirstName = null;

        var viewModel = await _fixture.Map();

        using (new AssertionScope())
        {
            viewModel.ApprenticeshipUpdates.LastName.Should().Be(_fixture.ApprenticeshipUpdate.LastName);
            viewModel.ApprenticeshipUpdates.FirstName.Should().Be(_fixture.GetReviewApprenticeshipUpdatesResponse.OriginalApprenticeship.FirstName);
        }
    }

    [Test]
    public async Task If_BothNames_Updated_Map_BothNames_From_Update()
    {
        var viewModel = await _fixture.Map();

        using (new AssertionScope())
        {
            viewModel.ApprenticeshipUpdates.FirstName.Should().Be(_fixture.ApprenticeshipUpdate.FirstName);
            viewModel.ApprenticeshipUpdates.LastName.Should().Be(_fixture.ApprenticeshipUpdate.LastName);
        }
    }

    [Test]
    public async Task OriginalApprenticeship_Email_IsMapped()
    {
        var viewModel = await _fixture.Map();

        viewModel.ApprenticeshipUpdates.Email.Should().Be(_fixture.ApprenticeshipUpdate.Email);
    }

    public class ReviewApprenticeshipUpdatesRequestToViewModelMapperTestsFixture
    {
        public ReviewApprenticeshipUpdatesRequestToViewModelMapper Mapper;
        public ReviewApprenticeshipUpdatesRequest Source;
        public Mock<ICommitmentsApiClient> CommitmentApiClient;
        public Mock<IOuterApiClient> ApiClient;
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
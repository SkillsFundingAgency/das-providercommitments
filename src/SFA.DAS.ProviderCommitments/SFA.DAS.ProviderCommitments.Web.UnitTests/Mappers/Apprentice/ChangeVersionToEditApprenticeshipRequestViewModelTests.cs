﻿using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ChangeVersionToEditApprenticeshipRequestViewModelTests
    {
        private Fixture _fixture;
        private ChangeVersionViewModel _viewModel;

        private GetApprenticeshipResponse _getApprenticeshipResponse;
        private GetPriceEpisodesResponse _getPriceEpisodesResponse;
        private GetTrainingProgrammeResponse _getVersionResponse;

        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;

        private ChangeVersionToEditApprenticeshipRequestViewModelMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _viewModel = _fixture.Create<ChangeVersionViewModel>();
            _getApprenticeshipResponse = _fixture.Create<GetApprenticeshipResponse>();

            var priceEpisode = _fixture.Build<PriceEpisode>()
                .With(x => x.ApprenticeshipId, _getApprenticeshipResponse.Id)
                .With(x => x.FromDate, _getApprenticeshipResponse.StartDate.Value.AddDays(-1))
                .Without(x => x.ToDate)
                .Create();

            _getPriceEpisodesResponse = _fixture.Build<GetPriceEpisodesResponse>()
                .With(x => x.PriceEpisodes, new List<PriceEpisode> { priceEpisode })
                .Create();

            var trainingProgramme = _fixture.Build<TrainingProgramme>().With(x => x.Name, _getApprenticeshipResponse.CourseName).Create();

            _getVersionResponse = new GetTrainingProgrammeResponse { TrainingProgramme = trainingProgramme };

            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeship(_viewModel.ApprenticeshipId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getApprenticeshipResponse);

            _mockCommitmentsApiClient.Setup(c => c.GetPriceEpisodes(_viewModel.ApprenticeshipId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getPriceEpisodesResponse);

            _mockCommitmentsApiClient.Setup(c => c.GetTrainingProgrammeVersionByCourseCodeAndVersion(_getApprenticeshipResponse.CourseCode, _viewModel.SelectedVersion, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_getVersionResponse);

            _mapper = new ChangeVersionToEditApprenticeshipRequestViewModelMapper(_mockCommitmentsApiClient.Object);
        }

        [Test]
        public async Task When_TheNewVersionHasAnUpdatedName_Then_MapTheNewName()
        {
            _getVersionResponse.TrainingProgramme.Name = _fixture.Create<string>();

            var result = await _mapper.Map(_viewModel);

            result.TrainingName.Should().Be(_getVersionResponse.TrainingProgramme.Name);
        }

        [Test]
        public async Task When_TheNewVersionHasTheSameName_Then_MapCurrentTrainingName()
        {
            var result = await _mapper.Map(_viewModel);

            result.TrainingName.Should().Be(_getApprenticeshipResponse.CourseName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Then_HasOptionsIsMappedCorrectly(bool hasOptions)
        {
            if (hasOptions == false)
            {
                _getVersionResponse.TrainingProgramme.Options = new List<string>();
            }

            var result = await _mapper.Map(_viewModel);

            result.HasOptions.Should().Be(hasOptions);
        }

        [Test]
        public async Task VerifyGetApprenticeship()
        {
            await _mapper.Map(_viewModel);

            _mockCommitmentsApiClient.Verify(c => c.GetApprenticeship(_viewModel.ApprenticeshipId, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task VerifyGetPriceEpisodes()
        {
            await _mapper.Map(_viewModel);

            _mockCommitmentsApiClient.Verify(c => c.GetPriceEpisodes(_viewModel.ApprenticeshipId, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task VerifyViewModelIsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            result.Version.Should().Be(_viewModel.SelectedVersion);
            result.ProviderId.Should().Be(_viewModel.ProviderId);
            result.ApprenticeshipHashedId.Should().Be(_viewModel.ApprenticeshipHashedId);

            result.ULN.Should().Be(_getApprenticeshipResponse.Uln);
            result.FirstName.Should().Be(_getApprenticeshipResponse.FirstName);
            result.LastName.Should().Be(_getApprenticeshipResponse.LastName);
            result.Email.Should().Be(_getApprenticeshipResponse.Email);
            result.CourseCode.Should().Be(_getApprenticeshipResponse.CourseCode);
            result.ProviderReference.Should().Be(_getApprenticeshipResponse.ProviderReference);

            result.Cost.Should().Be(_getPriceEpisodesResponse.PriceEpisodes.GetPrice());
        }
    }
}

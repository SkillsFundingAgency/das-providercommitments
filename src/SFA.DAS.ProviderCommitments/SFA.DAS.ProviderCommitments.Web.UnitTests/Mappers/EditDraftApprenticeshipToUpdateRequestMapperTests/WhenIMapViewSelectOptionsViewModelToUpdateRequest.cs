using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.EditDraftApprenticeshipToUpdateRequestMapperTests
{
    public class WhenIMapViewSelectOptionsViewModelToUpdateRequest
    {
        private ViewSelectOptionsViewModel _source;
        private ViewSelectOptionsViewModelToUpdateRequestMapper _mapper;
        private Func<Task<UpdateDraftApprenticeshipApimRequest>> _act;
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private GetDraftApprenticeshipResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<ViewSelectOptionsViewModel>()
                .Create();
            _apiResponse = fixture.Build<GetDraftApprenticeshipResponse>().Create();
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient
                .Setup(x => x.GetDraftApprenticeship(_source.CohortId, _source.DraftApprenticeshipId,
                    CancellationToken.None)).ReturnsAsync(_apiResponse);
            _mapper = new ViewSelectOptionsViewModelToUpdateRequestMapper(_commitmentsApiClient.Object);
            
            _act = async () => await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public async Task ThenTheApiResponseIsMappedToRequest()
        {
            var result = await _act();
            
            result.Should().BeEquivalentTo(_apiResponse, 
                options=> options.ExcludingMissingMembers()
                    .Excluding(c=>c.StartDate)
                    .Excluding(c=>c.EndDate)
                    .Excluding(c=>c.DateOfBirth)
                    .Excluding(c=>c.StandardUId)
                    .Excluding(c=>c.DeliveryModel)
            );
            result.CourseOption.Should().Be(_source.SelectedOption);
        }

        [Test]
        public async Task ThenIfTheSelectedOptionIsMinusOneThenEmptyStringSet()
        {
            _source.SelectedOption = "-1";
            
            var result = await _act();
            
            result.CourseOption.Should().BeEmpty();
        }
    }
}
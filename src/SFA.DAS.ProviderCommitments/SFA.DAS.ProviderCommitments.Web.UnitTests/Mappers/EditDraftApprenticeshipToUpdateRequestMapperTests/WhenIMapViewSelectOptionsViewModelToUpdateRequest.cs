using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.EditDraftApprenticeshipToUpdateRequestMapperTests
{
    public class WhenIMapViewSelectOptionsViewModelToUpdateRequest
    {
        private ViewSelectOptionsViewModel _source;
        private ViewSelectOptionsViewModelToUpdateRequestMapper _mapper;
        private Func<Task<UpdateDraftApprenticeshipApimRequest>> _act;
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private GetEditDraftApprenticeshipResponse _apiResponse;
        private Mock<IOuterApiClient> _outerApiClient;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _source = fixture.Build<ViewSelectOptionsViewModel>()
                .Create();
            _apiResponse = fixture.Build<GetEditDraftApprenticeshipResponse>().Create();
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _outerApiClient = new Mock<IOuterApiClient>();
            _outerApiClient
                .Setup(x => x.Get<GetEditDraftApprenticeshipResponse>(It.IsAny< GetEditDraftApprenticeshipRequest>())).ReturnsAsync(_apiResponse);

            _mapper = new ViewSelectOptionsViewModelToUpdateRequestMapper(_outerApiClient.Object);
            
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
                    .Excluding(c => c.HasLearnerDataChanges)
                    .Excluding(c => c.LastLearnerDataSync)
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
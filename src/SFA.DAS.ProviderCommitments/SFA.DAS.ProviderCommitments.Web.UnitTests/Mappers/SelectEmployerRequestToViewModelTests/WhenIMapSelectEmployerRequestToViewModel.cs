using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderRelationships.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.SelectEmployerRequestToViewModelTests
{
    [TestFixture]
    public class WhenIMapSelectEmployerRequestToViewModel
    {
        private IMapper<SelectEmployerRequest, SelectEmployerViewModel> _mapper;
        private SelectEmployerRequest _request;
        private Mock<IProviderRelationshipsApiClient> _mockProviderRelationshipsApiClient;
        private Func<Task<SelectEmployerViewModel>> _act;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _request = fixture.Build<SelectEmployerRequest>()
                .With(x => x.ProviderId)
                .Create();

            _mockProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();
            _mapper = new SelectEmployerViewModelMapper(_mockProviderRelationshipsApiClient.Object);

            _act = async () => await _mapper.Map(_request);
        }

        [Test]
        public async Task ThenCallsProviderRelationshipsApiClient()
        {

        }

    }
}

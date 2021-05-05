using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
    [TestFixture]
    public class IDraftApprenticeshipDetailsViewModelMapperTests
    {
        private IDraftApprenticeshipDetailsViewModelMapper _mapper;
        private Mock<ICommitmentsApiClient> _apiClient;
        private Mock<IModelMapper> _modelMapper;
        private GetCohortResponse _cohort;
        private DraftApprenticeshipRequest _request;

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();
            _cohort = autoFixture.Create<GetCohortResponse>();

            _request = autoFixture.Create<DraftApprenticeshipRequest>();

            _apiClient = new Mock<ICommitmentsApiClient>();

            _apiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cohort);

            _modelMapper = new Mock<IModelMapper>();

            _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(It.IsAny<EditDraftApprenticeshipRequest>()))
                .ReturnsAsync(new EditDraftApprenticeshipViewModel());

            _modelMapper.Setup(x => x.Map<IDraftApprenticeshipViewModel>(It.IsAny<ViewDraftApprenticeshipRequest>()))
                .ReturnsAsync(new ViewDraftApprenticeshipViewModel());

            _mapper = new IDraftApprenticeshipDetailsViewModelMapper(_apiClient.Object, _modelMapper.Object);
        }

        [TestCase(Party.Provider, typeof(EditDraftApprenticeshipViewModel))]
        [TestCase(Party.Employer, typeof(ViewDraftApprenticeshipViewModel))]
        [TestCase(Party.TransferSender, typeof(ViewDraftApprenticeshipViewModel))]
        public async Task When_Mapping_The_Mapping_Request_Is_Passed_On_To_The_Appropriate_Mapper(Party withParty, Type expectedViewModelType)
        {
            _cohort.WithParty = withParty;
            var viewModel = await _mapper.Map(_request);

            viewModel.GetType().Should().Be(expectedViewModelType);
        }
    }
}

﻿using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenIMapCreateCohortWithAddDraftApprenticeshipViewModelToSelectDeliveryModelViewModel
{
    private SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper _mapper;
    private Mock<ISelectDeliveryModelMapperHelper> _helper;
    private SelectDeliveryModelViewModel _model;
    private CreateCohortWithDraftApprenticeshipRequest _request;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _request = fixture.Create<CreateCohortWithDraftApprenticeshipRequest>();
        _model = fixture.Create<SelectDeliveryModelViewModel>();

        _helper = new Mock<ISelectDeliveryModelMapperHelper>();
        _helper.Setup(x => x.Map(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DeliveryModel?>(), It.IsAny<bool?>())).ReturnsAsync(_model);

        _mapper = new SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(_helper.Object);
    }

    [Test]
    public async Task TheParamsArePassedInCorrectly()
    {
        await _mapper.Map(_request);
        _helper.Verify(x=>x.Map(_request.ProviderId, _request.CourseCode, _request.AccountLegalEntityId, _request.DeliveryModel, _request.IsOnFlexiPaymentPilot));
    }

    [Test]
    public async Task ThenModelIsReturned()
    {
        var result = await _mapper.Map(_request);
        result.Should().Be(_model);
    }
}
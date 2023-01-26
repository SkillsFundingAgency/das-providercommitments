using AutoFixture;
using NUnit.Framework;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers

{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelFromEditDraftApprenticeshipViewModel 
    {
        private SelectDeliveryModelViewModelFromEditDraftApprenticeshipViewModelMapper _mapper;
        private Mock<ISelectDeliveryModelMapperHelper> _helper;
        private SelectDeliveryModelViewModel _model;
        private EditDraftApprenticeshipViewModel _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Build<EditDraftApprenticeshipViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartDate).Without(x => x.StartMonth).Without(x => x.StartYear)
                .Without(x => x.EndMonth).Without(x => x.EndYear).Create();
            _model = fixture.Create<SelectDeliveryModelViewModel>();

            _helper = new Mock<ISelectDeliveryModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DeliveryModel?>(), It.IsAny<bool?>())).ReturnsAsync(_model);

            _mapper = new SelectDeliveryModelViewModelFromEditDraftApprenticeshipViewModelMapper(_helper.Object);
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
            Assert.AreEqual(_model, result);
        }
    }
}

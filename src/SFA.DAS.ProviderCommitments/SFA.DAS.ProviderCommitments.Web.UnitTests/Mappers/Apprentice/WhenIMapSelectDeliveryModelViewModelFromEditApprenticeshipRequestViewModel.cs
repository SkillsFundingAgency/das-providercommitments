using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice

{
    [TestFixture]
    public class WhenIMapSelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModel
    {
        private SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper _mapper;
        private Mock<ISelectDeliveryModelMapperHelper> _helper;
        private SelectDeliveryModelViewModel _model;
        private EditApprenticeshipRequestViewModel _request;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _request = fixture.Build<EditApprenticeshipRequestViewModel>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
                .Without(x => x.StartDate).Without(x => x.StartMonth).Without(x => x.StartYear)
                .Without(x => x.EndDate).Without(x => x.EndMonth).Without(x => x.EndYear).Create();

            _model = fixture.Create<SelectDeliveryModelViewModel>();

            _helper = new Mock<ISelectDeliveryModelMapperHelper>();
            _helper.Setup(x => x.Map(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<DeliveryModel?>())).ReturnsAsync(_model);

            _mapper = new SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper(_helper.Object);
        }

        [Test]
        public async Task TheParamsArePassedInCorrectly()
        {
            var result = await _mapper.Map(_request);
            _helper.Verify(x=>x.Map(_request.ProviderId, _request.CourseCode, _request.DeliveryModel));
       }

        [Test]
        public async Task ThenModelIsReturned()
        {
            var result = await _mapper.Map(_request);
            Assert.AreEqual(_model, result);
        }
    }
}

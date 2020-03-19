using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ChangeOfEmployerViewModelMapperTests
    {
        private ChangeOfEmployerViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ChangeOfEmployerViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.request.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenOldEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.EmployerName, result.OldEmployerName);
        }

        [Test]
        public async Task ThenApprenticeNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual($"{_fixture.getApprenticeshipResponse.FirstName} {_fixture.getApprenticeshipResponse.LastName}", result.ApprenticeName);
        }

        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.StopDate, result.StopDate);
        }

        [Test]
        public async Task ThenOldStartDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.StartDate, result.OldStartDate);
        }

        [Test]
        public async Task ThenOldPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.Cost, result.OldPrice);
        }

        [Test]
        public async Task ThenNewEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.accountLegalEntityResponse.LegalEntityName, result.NewEmployerName);
        }

        [Test]
        public async Task ThenNewStartDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedStartDate = new MonthYearModel(_fixture.request.StartDate);

            Assert.AreEqual(expectedStartDate.MonthYear, result.NewStartDate.MonthYear);
        }

        [Test]
        public async Task ThenNewPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.request.Price, result.NewPrice);
        }

        [Test]
        public async Task ThenFundingBandCapIsMapped()
        {
            var priceBand = 1000;
            _fixture.SetStartDateInRequest(DateTime.Now).SetPriceBand(priceBand, DateTime.Now);
            var result = await _fixture.Map();

            Assert.AreEqual(priceBand, result.FundingBandCap);
        }

        [TestCase(1000, 900, true)]
        [TestCase(800, 900, false)]
        [TestCase(800, 800, false)]
        public async Task ThenExceedsFundingBandCapIsMapped(int price, int fundingCap, bool fundingCapExceeded)
        {
            _fixture.SetPriceInRequest(price).SetStartDateInRequest(DateTime.Now).SetPriceBand(fundingCap, DateTime.Now);
            var result = await _fixture.Map();

            Assert.AreEqual(result.ExceedsFundingBandCap, fundingCapExceeded);
        }
    }

    public class ChangeOfEmployerViewModelMapperFixture
    {
        private readonly ChangeOfEmployerViewModelMapper _sut;

        public ChangeOfEmployerRequest request { get; }

        public ITrainingProgramme trainingProgramme;
        public GetApprenticeshipResponse getApprenticeshipResponse { get; set; }
        public AccountLegalEntityResponse accountLegalEntityResponse { get; set; }

        public ChangeOfEmployerViewModelMapperFixture()
        {
            Fixture fixture = new Fixture();
            request = fixture.Create<ChangeOfEmployerRequest>();
            request.StartDate = "012020";
            getApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            trainingProgramme = fixture.Create<Standard>();
            accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();

            var commitmentAiClient = new Mock<ICommitmentsApiClient>();
            var trainingProgrammeApiClient = new Mock<ITrainingProgrammeApiClient>();

            commitmentAiClient.Setup(x => x.GetApprenticeship(request.ApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(() => getApprenticeshipResponse);
            commitmentAiClient.Setup(x => x.GetLegalEntity(getApprenticeshipResponse.AccountLegalEntityId, It.IsAny<CancellationToken>())).ReturnsAsync(() => accountLegalEntityResponse);
            trainingProgrammeApiClient.Setup(y => y.GetTrainingProgramme(getApprenticeshipResponse.CourseCode)).ReturnsAsync(() => trainingProgramme);

            _sut = new ChangeOfEmployerViewModelMapper(commitmentAiClient.Object, trainingProgrammeApiClient.Object, Mock.Of<ILogger<ChangeOfEmployerViewModelMapper>>());
        }

        public ChangeOfEmployerViewModelMapperFixture SetPriceBand(int fundingCap, DateTime startDate)
        {
            trainingProgramme = new Standard
            {
                FundingPeriods = new System.Collections.Generic.List<FundingPeriod>
                {
                    new FundingPeriod
                    {
                          EffectiveFrom =startDate.AddDays(-1),
                          EffectiveTo = startDate.AddDays(1),
                          FundingCap = fundingCap
                    }
                }
            };

            return this;
        }

        public ChangeOfEmployerViewModelMapperFixture SetStartDateInRequest(DateTime startDate)
        {
            request.StartDate = startDate.Month.ToString() + startDate.Year.ToString();

            return this;
        }

        public ChangeOfEmployerViewModelMapperFixture SetPriceInRequest(int price)
        {
            request.Price = price;
            return this;
        }

        public Task<ChangeOfEmployerViewModel> Map() => _sut.Map(request);
    }
}

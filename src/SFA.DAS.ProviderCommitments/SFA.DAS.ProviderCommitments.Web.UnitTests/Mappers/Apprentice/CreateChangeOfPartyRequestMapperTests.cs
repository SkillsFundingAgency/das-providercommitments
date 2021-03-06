﻿using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class CreateChangeOfPartyRequestMapperTests
    {
        private CreateChangeOfPartyRequestMapper _mapper;
        private ConfirmViewModel _source;
        private CreateChangeOfPartyRequestRequest _result;

        [SetUp]
        public async Task Arrange()
        {
            var fixture = new Fixture();

            _source = new ConfirmViewModel
            {
                NewStartDate = "032020",
                NewEndDate = "092021",
                AccountLegalEntityId = fixture.Create<long>(),
                NewPrice = fixture.Create<int>()
            };

            
            _mapper = new CreateChangeOfPartyRequestMapper();
            _result = await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public void ChangeOfPartyRequestTypeIsMappedCorrectly()
        {
            Assert.AreEqual(ChangeOfPartyRequestType.ChangeEmployer, _result.ChangeOfPartyRequestType);
        }

        [Test]
        public void NewPartyIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.AccountLegalEntityId, _result.NewPartyId);
        }

        [Test]
        public void NewPriceIsMappedCorrectly()
        {
            Assert.AreEqual(_source.NewPrice, _result.NewPrice);
        }

        [Test]
        public void NewStartDateIsMappedCorrectly()
        {
            Assert.AreEqual(new MonthYearModel(_source.NewStartDate).Date, _result.NewStartDate);
        }

        [Test]
        public void NewEndDateIsMappedCorrectly()
        {
            Assert.AreEqual(new MonthYearModel(_source.NewEndDate).Date, _result.NewEndDate);
        }
    }
}

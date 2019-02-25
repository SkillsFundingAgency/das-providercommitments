using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Mappers;
using FundingPeriod = SFA.DAS.Apprenticeships.Api.Types.FundingPeriod;

namespace SFA.DAS.ProviderCommitments.Tests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapStandard
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private StandardSummary _standard;
        private Func<StandardsView> _act;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper(Mock.Of<ICurrentDateTime>());

            _standard = new StandardSummary
            {
                Id = "1",
                Title = "TestTitle",
                Level = 1,
                CurrentFundingCap = 1000, //this is to become redundant
                EffectiveFrom = new DateTime(2017, 05, 01),
                LastDateForNewStarts = new DateTime(2020, 7, 31),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod { EffectiveFrom = new DateTime(2017,05,01), EffectiveTo = new DateTime(2018, 12, 31), FundingCap = 5000 },
                    new FundingPeriod { EffectiveFrom = new DateTime(2019,01,01), EffectiveTo = new DateTime(2020, 7, 31), FundingCap = 2000 }
                }
            };

            //copy the payload to guard against handler modifications
            _act = () => _mapper.MapFrom(new StandardSummary[] { TestHelper.Clone(_standard) });
        }

        [Test]
        public void ThenTitleIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            var expectedTitle = $"{_standard.Title}, Level: {_standard.Level} (Standard)";
            Assert.AreEqual(expectedTitle, result.Standards.First().Title);
        }

        [Test]
        public void ThenEffectiveFromIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            Assert.AreEqual(_standard.EffectiveFrom, result.Standards.First().EffectiveFrom);
        }

        [Test]
        public void ThenFundingPeriodsAreOrderedAscending()
        {
            _standard.FundingPeriods = new List<FundingPeriod>
            {
                new FundingPeriod {EffectiveFrom = new DateTime(2019,1,1)},
                new FundingPeriod {EffectiveFrom = null},
                new FundingPeriod {EffectiveFrom = new DateTime(2018,1,1)}
            };

            var result = _act();

            //Assert
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new List<FundingPeriod>
            {
                new FundingPeriod {EffectiveFrom = null},
                new FundingPeriod {EffectiveFrom = new DateTime(2018,1,1)},
                new FundingPeriod {EffectiveFrom = new DateTime(2019,1,1)}
            }, result.Standards.First().FundingPeriods));
        }

        [Test]
        public void ThenEffectiveToIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            Assert.AreEqual(_standard.LastDateForNewStarts, result.Standards.First().EffectiveTo);
        }

        [Test]
        public void ThenFundingPeriodsAreMappedCorrectly()
        {
            var result = _act();

            Assert.IsTrue(TestHelper.EnumerablesAreEqual(_standard.FundingPeriods, result.Standards.First().FundingPeriods));
        }

        [Test]
        public void ThenFundingPeriodsAreMappedCorrectlyWhenNull()
        {
            //Arrange
            _standard.FundingPeriods = null;

            var result = _act();

            //Assert
            Assert.IsNotNull(result.Standards.First().FundingPeriods);
            Assert.IsEmpty(result.Standards.First().FundingPeriods);
        }
    }
}
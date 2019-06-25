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

namespace SFA.DAS.ProviderCommitments.UnitTests.Mappers.ApprenticeshipInfoServiceMapperTests
{
    [TestFixture]
    public class WhenIMapFramework
    {
        private ApprenticeshipInfoServiceMapper _mapper;
        private FrameworkSummary _framework;
        private Func<FrameworksView> _act;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipInfoServiceMapper(Mock.Of<ICurrentDateTime>());

            _framework = new FrameworkSummary
            {
                Id = "1",
                Title = "TestTitle",
                FrameworkName = "TestFrameworkName",
                PathwayName = "TestPathwayName",
                Level = 1,
                CurrentFundingCap = 1000, //this is to become redundant
                EffectiveFrom = new DateTime(2017, 05, 01),
                EffectiveTo = new DateTime(2020, 7, 31),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod { EffectiveFrom = new DateTime(2017,05,01), EffectiveTo = new DateTime(2018, 12, 31), FundingCap = 5000 },
                    new FundingPeriod { EffectiveFrom = new DateTime(2019,01,01), EffectiveTo = new DateTime(2020, 7, 31), FundingCap = 2000 }
                }
            };

            _act = () => _mapper.MapFrom(new FrameworkSummary[] { TestHelper.Clone(_framework) });
        }

        [Test]
        public void ThenTitleIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            var expectedTitle = $"{_framework.Title}, Level: {_framework.Level}";
            Assert.AreEqual(expectedTitle, result.Frameworks.First().Title);
        }

        [Test]
        public void ThenEffectiveFromIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            Assert.AreEqual(_framework.EffectiveFrom, result.Frameworks.First().EffectiveFrom);
        }

        [Test]
        public void ThenFundingPeriodsAreOrderedAscending()
        {
            _framework.FundingPeriods = new List<FundingPeriod>
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
            }, result.Frameworks.First().FundingPeriods));
        }

        [Test]
        public void ThenEffectiveToIsMappedCorrectly()
        {
            var result = _act();

            //Assert
            Assert.AreEqual(_framework.EffectiveFrom, result.Frameworks.First().EffectiveFrom);
        }

        [Test]
        public void ThenFundingPeriodsAreMappedCorrectly()
        {
            var result = _act();

            Assert.IsTrue(TestHelper.EnumerablesAreEqual(_framework.FundingPeriods, result.Frameworks.First().FundingPeriods));
        }

        [Test]
        public void ThenFundingPeriodsAreMappedCorrectlyWhenNull()
        {
            //Arrange
            _framework.FundingPeriods = null;

            var result = _act();

            //Assert
            Assert.IsNotNull(result.Frameworks.First().FundingPeriods);
            Assert.IsEmpty(result.Frameworks.First().FundingPeriods);
        }
    }
}
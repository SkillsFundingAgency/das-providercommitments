using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;

namespace SFA.DAS.ProviderCommitments.Tests.Queries.GetTrainingCoursesTests
{
    [TestFixture]
    public class WhenIGetTrainingCourses
    {
        private GetTrainingCoursesQueryHandler _handler;

        private Mock<IApprenticeshipInfoService> _apprenticeshipInfoServiceWrapper;

        private List<Standard> _standards;
        private List<Framework> _frameworks;

        [SetUp]
        public void Arrange()
        {
            _standards = new List<Standard>
            {
                new Standard
                {
                    EffectiveFrom = new DateTime(2016,01,01),
                    EffectiveTo = new DateTime(2016,12,31)
                }
            };
            _frameworks = new List<Framework>
            {
                new Framework
                {
                    EffectiveFrom = new DateTime(2017,01,01),
                    EffectiveTo = new DateTime(2017,12,31)
                }
            };

            _apprenticeshipInfoServiceWrapper = new Mock<IApprenticeshipInfoService>();

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetFrameworksAsync(It.IsAny<bool>()))
                .ReturnsAsync(new FrameworksView
                {
                    CreatedDate = DateTime.UtcNow,
                    Frameworks = _frameworks
                });

            _apprenticeshipInfoServiceWrapper.Setup(x => x.GetStandardsAsync(It.IsAny<bool>()))
                .ReturnsAsync(new StandardsView
                {
                    CreationDate = DateTime.UtcNow.Date,
                    Standards = _standards
                });

            _handler = new GetTrainingCoursesQueryHandler(_apprenticeshipInfoServiceWrapper.Object);
        }

        [Test]
        public async Task ThenOnlyStandardsAreReturnedIfIExcludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = false,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(_standards.Count, result.TrainingCourses.Count);
            Assert.IsInstanceOf<Standard>(result.TrainingCourses[0]);
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(_standards.Count + _frameworks.Count, result.TrainingCourses.Count);
            Assert.IsTrue(result.TrainingCourses.Any(x => x is Standard));
            Assert.IsTrue(result.TrainingCourses.Any(x => x is Framework));
        }

        [Test]
        public async Task ThenIfISpecifyAnEffectiveDateIOnlyGetCoursesActiveOnThatDay()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = new DateTime(2016, 06, 01)
            }, new CancellationToken());

            Assert.AreEqual(1, result.TrainingCourses.Count);
            Assert.IsInstanceOf<Standard>(result.TrainingCourses[0]);
        }

        [Test]
        public async Task ThenIfIDoNotSpecifyAnEffectiveDateIGetAllCourses()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(2, result.TrainingCourses.Count);
        }
    }
}
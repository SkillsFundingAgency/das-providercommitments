using System;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ValidateChangeOfEmployerOverlapApimRequestMapperTests
    {
        private ValidateChangeOfEmployerOverlapApimRequestMapper mapper;
        TrainingDatesViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            var baseDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var startDate = baseDate;
            var endDate = baseDate.AddYears(2);

            var fixture = new Fixture();

            viewModel = fixture.Build<TrainingDatesViewModel>()
                //.With(x => x.DetailsAcknowledgement, true)
                .With(x => x.StartDate, new MonthYearModel(startDate.ToString("MMyyyy")))
                .With(x => x.EndDate, new MonthYearModel(endDate.ToString("MMyyyy")))
                .With(x => x.EmploymentEndDate, new MonthYearModel(endDate.ToString("MMyyyy")))
                .Create();

            mapper = new ValidateChangeOfEmployerOverlapApimRequestMapper();
        }

        [Test, MoqAutoData]
        public async Task Uln_IsMapped()
        {
            var result = await mapper.Map(viewModel);
            Assert.AreEqual(viewModel.Uln, result.Uln);
        }

        [Test, MoqAutoData]
        public async Task ProviderId_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.ProviderId, result.ProviderId);
        }

        [Test, MoqAutoData]
        public async Task StartDate_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.StartDate.Date.Value.ToString("dd-MM-yyyy"), result.StartDate);
        }

        [Test, MoqAutoData]
        public async Task EndDate_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.EndDate.Date.Value.ToString("dd-MM-yyyy"), result.EndDate);
        }
    }
}
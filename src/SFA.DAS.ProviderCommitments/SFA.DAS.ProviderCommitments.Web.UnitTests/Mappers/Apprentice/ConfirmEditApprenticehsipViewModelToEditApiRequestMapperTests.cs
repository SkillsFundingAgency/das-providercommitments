using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ConfirmEditApprenticeshipViewModelToEditApiRequestMapperTests
    {
        private ConfirmEditApprenticeshipViewModelToEditApiRequestMapper _mapper;
        ConfirmEditApprenticeshipViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            _viewModel = fixture.Build<ConfirmEditApprenticeshipViewModel>()
                 .With(x => x.StartMonth, DateTime.Now.Month)
                 .With(x => x.StartYear, DateTime.Now.Year)
                 .With(x => x.EndMonth, DateTime.Now.Month)
                 .With(x => x.EndYear, DateTime.Now.Year)
                 .With(x => x.EmploymentEndMonth, DateTime.Now.Month)
                 .With(x => x.EmploymentEndYear, DateTime.Now.Year)
                 .With(x => x.BirthMonth, DateTime.Now.Month)
                 .With(x => x.BirthYear, DateTime.Now.Year)
                 .With(x => x.BirthDay, DateTime.Now.Day)
                 .Create();

            _mapper = new ConfirmEditApprenticeshipViewModelToEditApiRequestMapper();
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);
            Assert.That(result.ApprenticeshipId, Is.EqualTo(_viewModel.ApprenticeshipId));
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.FirstName, Is.EqualTo(_viewModel.FirstName));
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.LastName, Is.EqualTo(_viewModel.LastName));
        }

        [Test]
        public async Task Email_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.Email, Is.EqualTo(_viewModel.Email));
        }

        [Test]
        public async Task Dob_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.DateOfBirth, Is.EqualTo(_viewModel.DateOfBirth));
        }

        [Test]
        public async Task StartDate_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.StartDate, Is.EqualTo(_viewModel.StartDate));
        }


        [Test]
        public async Task EndDate_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.EndDate, Is.EqualTo(_viewModel.EndDate));
        }

        [Test, MoqAutoData]
        public async Task DeliveryModel_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.DeliveryModel, Is.EqualTo(_viewModel.DeliveryModel));
        }

        [Test, MoqAutoData]
        public async Task EmploymentEndDate_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.EmploymentEndDate, Is.EqualTo(_viewModel.EmploymentEndDate));
        }

        [Test, MoqAutoData]
        public async Task EmploymentPrice_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.EmploymentPrice, Is.EqualTo(_viewModel.EmploymentPrice));
        }

        [Test]
        public async Task Course_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.CourseCode, Is.EqualTo(_viewModel.CourseCode));
        }

        [Test]
        public async Task Cost_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.Cost, Is.EqualTo(_viewModel.Cost));
        }

        [Test]
        public async Task Version_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.Version, Is.EqualTo(_viewModel.Version));
        }

        [Test, MoqAutoData]
        public async Task Option_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.Option, Is.EqualTo(_viewModel.Option));
        }

        [Test]
        public async Task When_OptionIsTBC_Option_IsMapped()
        {
            _viewModel.Option = "TBC";
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.Option, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Reference_IsMapped()
        {
            var result = await _mapper.Map(_viewModel);

            Assert.That(result.ProviderReference, Is.EqualTo(_viewModel.ProviderReference));
        }
    }
}

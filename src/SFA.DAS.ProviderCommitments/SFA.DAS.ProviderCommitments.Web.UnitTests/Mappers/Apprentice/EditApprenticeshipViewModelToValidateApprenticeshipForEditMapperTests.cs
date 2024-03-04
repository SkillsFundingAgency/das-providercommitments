using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests
    {
        private EditApprenticeshipRequestViewModel _request;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();
            fixture.Customize(new DateCustomisation());
            _request = fixture.Create<EditApprenticeshipRequestViewModel>();
        }

        [Test, MoqAutoData]
        public async Task ApprenticeshipId_IsMapped(
          EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.ApprenticeshipId, Is.EqualTo(_request.ApprenticeshipId));
        }

        [Test, MoqAutoData]
        public async Task FirstName_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.FirstName, Is.EqualTo(_request.FirstName));
        }

        [Test, MoqAutoData]
        public async Task LastName_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.LastName, Is.EqualTo(_request.LastName));
        }

        [Test, MoqAutoData]
        public async Task Email_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.Email, Is.EqualTo(_request.Email));
        }

        [Test, MoqAutoData]
        public async Task DateOfBirth_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.DateOfBirth, Is.EqualTo(_request.DateOfBirth.Date));
        }

        [Test, MoqAutoData]
        public async Task ULN_IsMapped(
              EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.ULN, Is.EqualTo(_request.ULN));
        }

        [Test, MoqAutoData]
        public async Task ProviderId_IsMapped(
              EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.ProviderId, Is.EqualTo(_request.ProviderId));
        }

        [Test, MoqAutoData]
        public async Task Cost_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.Cost, Is.EqualTo(_request.Cost));
        }

        [Test, MoqAutoData]
        public async Task ProviderReference_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.ProviderReference, Is.EqualTo(_request.ProviderReference));
        }

        [Test, MoqAutoData]
        public async Task StartDate_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.StartDate, Is.EqualTo(_request.StartDate.Date));
        }

        [Test, MoqAutoData]
        public async Task EndDate_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.EndDate, Is.EqualTo(_request.EndDate.Date));
        }

        [Test, MoqAutoData]
        public async Task DeliveryModel_IsMapped(
            EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.DeliveryModel, Is.EqualTo(_request.DeliveryModel));
        }

        [Test, MoqAutoData]
        public async Task CourseCode_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.TrainingCode, Is.EqualTo(_request.CourseCode));
        }

        [Test, MoqAutoData]
        public async Task Version_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.Version, Is.EqualTo(_request.Version));
        }

        [Test, MoqAutoData]
        public async Task Option_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.Option, Is.EqualTo(_request.Option));
        }

        [Test, MoqAutoData]
        public async Task WhenOptionIsTBC_OptionIsMappedToEmptyString(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            _request.Option = "TBC";

            var result = await mapper.Map(_request);

            Assert.That(result.Option, Is.EqualTo(string.Empty));
        }

        [Test, MoqAutoData]
        public async Task EmploymentEndDate_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.EmploymentEndDate, Is.EqualTo(_request.EmploymentEndDate.Date));
        }

        [Test, MoqAutoData]
        public async Task EmploymentPrice_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(_request);

            Assert.That(result.EmploymentPrice, Is.EqualTo(_request.EmploymentPrice));
        }

        public class DateCustomisation : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<EditApprenticeshipRequestViewModel>(composer =>
                    composer.Without(p => p.DateOfBirth)
                            .Without(p => p.EndDate)
                            .Without(p => p.StartDate)
                            .With(p => p.BirthDay, 1)
                            .With(p => p.BirthMonth, 12)
                            .With(p => p.BirthYear, 2000)
                            .With(p => p.StartMonth, 4)
                            .With(p => p.StartYear, 2019)
                            .With(p => p.EndMonth, 6)
                            .With(p => p.EndYear, 2021)
                    );
            }
        }
    }
}

using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ManagedFilterModel
{
    public class WhenGettingPagedRecordsFrom
    {
        [Test, AutoData]
        public void And_PageNumber_1_Then_Should_Be_1(ManageApprenticesFilterModel filterModel)
        {
            filterModel.PageNumber = 1;

            filterModel.PagedRecordsFrom.Should().Be(1);
        }

        [Test, AutoData]
        public void And_PageNumber_2_Then_Should_Be_PageSize_Plus_1(ManageApprenticesFilterModel filterModel)
        {
            filterModel.PageNumber = 2;

            filterModel.PagedRecordsFrom.Should().Be(ManageApprenticesFilterModel.PageSize+1);
        }

        [Test, AutoData]
        public void And_PageNumber_3_Then_Should_Be_Double_PageSize_Plus_1(ManageApprenticesFilterModel filterModel)
        {
            filterModel.PageNumber = 3;

            filterModel.PagedRecordsFrom.Should().Be(2 * ManageApprenticesFilterModel.PageSize+1);
        }

        [Test, AutoData]
        public void And_PageNumber_1_And_0_Records_Found_Then_Should_Be_0(ManageApprenticesFilterModel filterModel)
        {
            filterModel.PageNumber = 1;
            filterModel.TotalNumberOfApprenticeshipsFound = 0;

            filterModel.PagedRecordsFrom.Should().Be(0);
        }
    }
}
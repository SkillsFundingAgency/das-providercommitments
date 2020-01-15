using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ManageApprenticesFilterModelTests
{
    public class WhenGettingSearchOrFiltersApplied
    {
        [Test]
        public void And_Has_SearchTerm_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SearchTerm = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_Has_SelectedEmployer_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SelectedEmployer = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_Has_SelectedCourse_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SelectedCourse = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_Has_SelectedStatus_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SelectedStatus = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_Has_SelectedStartDate_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SelectedStartDate = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_Has_SelectedEndDate_Then_True()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SelectedEndDate = "asedfas"
            };

            filterModel.SearchOrFiltersApplied.Should().BeTrue();
        }

        [Test]
        public void And_No_Search_Or_Filter_Then_False()
        {
            var filterModel = new ManageApprenticesFilterModel();

            filterModel.SearchOrFiltersApplied.Should().BeFalse();
        }
    }
}
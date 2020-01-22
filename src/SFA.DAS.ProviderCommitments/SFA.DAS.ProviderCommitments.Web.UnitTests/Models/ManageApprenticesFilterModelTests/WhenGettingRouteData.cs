using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ManageApprenticesFilterModelTests
{
    public class WhenGettingRouteData
    {
        [Test]
        public void Then_Contains_Item_For_Each_Search_And_Filter_Value()
        {
            var filterModel = new ManageApprenticesFilterModel
            {
                SearchTerm = "asedfas",
                SelectedEmployer = "asdsad",
                SelectedCourse = "iknjso",
                SelectedStatus = "9psdgf",
                SelectedStartDate = DateTime.Today,
                SelectedEndDate = DateTime.Today
            };

            filterModel.RouteData.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                {"searchTerm", filterModel.SearchTerm },
                {"selectedEmployer", filterModel.SelectedEmployer},
                {"selectedCourse", filterModel.SelectedCourse},
                {"selectedStatus", filterModel.SelectedStatus},
                {"selectedStartDate", filterModel.SelectedStartDate.Value.ToString("yyyy-MM-dd")},
                {"selectedEndDate", filterModel.SelectedEndDate.Value.ToString("yyyy-MM-dd")}
            });
        }
    }
}
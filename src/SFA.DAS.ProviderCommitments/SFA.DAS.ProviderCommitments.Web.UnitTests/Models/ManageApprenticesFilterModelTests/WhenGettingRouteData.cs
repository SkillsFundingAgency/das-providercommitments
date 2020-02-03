using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ManageApprenticesFilterModelTests
{
    public class WhenGettingRouteData
    {
        [Test, AutoData]
        public void Then_Contains_Item_For_Each_Search_And_Filter_Value(
            ManageApprenticesFilterModel filterModel)
        {
            filterModel.RouteData.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                {nameof(filterModel.SearchTerm), filterModel.SearchTerm },
                {nameof(filterModel.SelectedEmployer), filterModel.SelectedEmployer},
                {nameof(filterModel.SelectedCourse), filterModel.SelectedCourse},
                {nameof(filterModel.SelectedStatus), filterModel.SelectedStatus.ToString()},
                {nameof(filterModel.SelectedStartDate), filterModel.SelectedStartDate.Value.ToString("yyyy-MM-dd")},
                {nameof(filterModel.SelectedEndDate), filterModel.SelectedEndDate.Value.ToString("yyyy-MM-dd")}
            });
        }

        [Test, AutoData]
        public void Then_Not_Contain_Item_For_PageNumber(
            ManageApprenticesFilterModel filterModel)
        {
            filterModel.RouteData.Should().NotContain(new KeyValuePair<string, string>(
                nameof(filterModel.PageNumber), filterModel.PageNumber.ToString() )
            );
        }
    }
}
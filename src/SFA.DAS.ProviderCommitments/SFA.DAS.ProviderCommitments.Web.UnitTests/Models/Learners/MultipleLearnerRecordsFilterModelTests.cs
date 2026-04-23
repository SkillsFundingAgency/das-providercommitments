using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Learners;

public class MultipleLearnerRecordsFilterModelTests
{
    private MultipleLearnerRecordsFilterModel _filterModel;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
        _filterModel = _fixture.Create<MultipleLearnerRecordsFilterModel>();
    }

    [Test]
    public void PageLink_Should_only_return_one_page_when_single_record()
    {
        _filterModel.TotalNumberOfLearnersFound = 1;
        _filterModel.PageNumber = 1;

        var result = _filterModel.PageLinks;

        result.Count().Should().Be(1);
    }

    [Test]
    public void PageLink_route_data_is_mapped()
    {
        _filterModel.TotalNumberOfLearnersFound = 1;
        _filterModel.PageNumber = 1;

        var result = _filterModel.PageLinks;

        result.Count().Should().Be(1);
        result.First().RouteData.Should().ContainKey(nameof(MultipleLearnerRecordsFilterModel.ProviderId));
        result.First().RouteData.Should().ContainKey(nameof(MultipleLearnerRecordsFilterModel.CacheKey));
        result.First().RouteData.Should().ContainKey("page");
        result.First().RouteData.Should().ContainValue(_filterModel.ProviderId.ToString());
        result.First().RouteData.Should().ContainValue(_filterModel.CacheKey.ToString());
        result.First().RouteData.Should().ContainValue(_filterModel.PageNumber.ToString());
    }

    [Test]
    public void PageLink_Should_not_include_previous_when_multiple_pages_and_current_page_is_one()
    {
        _filterModel.TotalNumberOfLearnersFound = 100;
        _filterModel.PageNumber = 1;

        var result = _filterModel.PageLinks;

        result.First().Should().NotBeNull();
        result.First().Label.Should().NotBe("Previous");
    }

    [Test]
    public void PageLink_Should_include_previous_when_multiple_pages_and_current_page_grater_than_one()
    {
        _filterModel.TotalNumberOfLearnersFound = 100;
        _filterModel.PageNumber = 2;

        var result = _filterModel.PageLinks;

        result.First().Should().NotBeNull();
        result.First().Label.Should().Be("Previous");
    }

    [Test]
    public void PageLink_Should_include_next_when_multiple_pages()
    {
        _filterModel.TotalNumberOfLearnersFound = 100;
        _filterModel.PageNumber = 1;

        var result = _filterModel.PageLinks;
        result.Last().Label.Should().Be("Next");
    }

    [Test]
    public void PageLink_Should_not_include_next_when_multiple_pages_and_last_page_selected()
    {
        _filterModel.TotalNumberOfLearnersFound = 100;
        _filterModel.PageNumber = 2;

        var result = _filterModel.PageLinks;
        result.Last().Label.Should().NotBe("Next");
    }

    [Test]
    public void PageLink_Should_include_max_seven_links_when_multiple_pages_present()
    {
        _filterModel.TotalNumberOfLearnersFound = 500;
        _filterModel.PageNumber = 4;

        var result = _filterModel.PageLinks;

        result.First().Label.Should().Be("Previous");
        result.Last().Label.Should().Be("Next");
        result.Count().Should().Be(7);
    }
}
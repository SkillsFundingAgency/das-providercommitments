using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Requests.Apprentices;

public class WhenBuildingGetAllChangeHistoryApiRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Constructed(long providerId, int pageNumber, int pageSize)
    {
        var actual = new GetAllChangeHistoryApiRequest(providerId, pageNumber, pageSize);

        actual.GetUrl.Should().Be($"provider/{providerId}/apprentices/change-history?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    [Test, AutoData]
    public void Then_Default_PageNumber_And_PageSize_Are_Used(long providerId)
    {
        var actual = new GetAllChangeHistoryApiRequest(providerId);

        actual.PageNumber.Should().Be(1);
        actual.PageSize.Should().Be(Constants.ChangeHistory.NumberOfChangesPerPage);
        actual.GetUrl.Should().Be($"provider/{providerId}/apprentices/change-history?pageNumber=1&pageSize={Constants.ChangeHistory.NumberOfChangesPerPage}");
    }
}

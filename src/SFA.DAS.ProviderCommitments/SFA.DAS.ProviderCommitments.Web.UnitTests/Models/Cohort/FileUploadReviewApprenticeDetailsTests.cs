using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Cohort;

public class FileUploadReviewApprenticeDetailsTests
{
    private ReviewApprenticeDetailsForFileUploadCohort _fileUploadReviewApprenticeDetails;

    [SetUp]
    public void Arrange()
    {
        _fileUploadReviewApprenticeDetails  = new ReviewApprenticeDetailsForFileUploadCohort();
    }

    [TestCase(1000, 900, true)]
    [TestCase(800, 900, false)]
    [TestCase(800, 800, false)]
    public void TestExceedsFundingBandCap(int price, int fundingCap, bool fundingCapExceeded)
    {
        //Act
        _fileUploadReviewApprenticeDetails.Price = price;
        _fileUploadReviewApprenticeDetails.FundingBandCap = fundingCap;

        //Assert
        var result = _fileUploadReviewApprenticeDetails.ExceedsFundingBandCap;
        result.Should().Be(fundingCapExceeded);
    }
}
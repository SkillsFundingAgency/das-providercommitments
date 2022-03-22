using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Cohort
{
    public class FileUploadReviewApprenticeDetailsTests
    {
        public ReviewApprenticeDetailsForFileUploadCohort FileUploadReviewApprenticeDetails;

        [SetUp]
        public void Arrange()
        {
            FileUploadReviewApprenticeDetails  = new ReviewApprenticeDetailsForFileUploadCohort();
        }

        [TestCase(1000, 900, true)]
        [TestCase(800, 900, false)]
        [TestCase(800, 800, false)]
        public void TestExceedsFundingBandCap(int price, int fundingCap, bool fundingCapExceeded)
        {
            //Act
            FileUploadReviewApprenticeDetails.Price = price;
            FileUploadReviewApprenticeDetails.FundingBandCap = fundingCap;

            //Assert
            var result = FileUploadReviewApprenticeDetails.ExceedsFundingBandCap;
            Assert.AreEqual(result, fundingCapExceeded);
        }
    }
}

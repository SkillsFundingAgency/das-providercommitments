using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ManageApprenticesFilterModelTests
{
    public class WhenGettingShowSearch
    {
        [TestCase(0, false, TestName = "No search results")]
        [TestCase(ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch - 1, false, TestName = "Less than required number by 1")]
        [TestCase(ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch, true, TestName = "Equal to required number")]
        [TestCase(ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 1, true, TestName = "Greater than required number by 1")]
        [TestCase(ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch + 100, true, TestName = "Greater than required number by 100")]
        public void Then_The_Show_Search_Flag_Is_Set_Based_On_Number_Of_Apprentices(int numberOfApprentices, bool expectedBool)
        {
            //Act
            var filterModel = new ManageApprenticesFilterModel
            {
                TotalNumberOfApprenticeships = numberOfApprentices
            };
            
            //Assert
            Assert.AreEqual(expectedBool, filterModel.ShowSearch);
        }
    }
}
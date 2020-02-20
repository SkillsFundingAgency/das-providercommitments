using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Apprentice
{
    public class IndexRequestTests
    {
        [TestCase(0,1)]
        [TestCase(1,1)]
        [TestCase(2,2)]
        [TestCase(-1,1)]
        public void Then_Page_Number_Is_Not_Set_Below_One(int actualPageNumber, int expectedPageNumber)
        {
            var request = new IndexRequest
            {
                PageNumber = actualPageNumber
            };

            Assert.AreEqual(expectedPageNumber, request.PageNumber);
        }
    }
}

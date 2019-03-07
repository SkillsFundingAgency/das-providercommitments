using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.HashingTemp;
using SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBindingTests
{
    public class AccountIdInfoExtractorTests 
    {
        [Test]
        public void BindModel_WithRouteAccountId_ShouldSetValueInHash()
        {
            // Arrange
            const long expectedAccountId = 456;
            const string hashedAccountId = "ABC123";

            var fixtures = new AccountIdInfoExtractorTestFixtures()
                .SetUnhashValue(hashedAccountId, expectedAccountId)
                .WithHashedAccountIdInRoute(hashedAccountId);

            // Act
            fixtures.RunValueExtractor();

            // Assert
            Assert.IsTrue(fixtures.UnhashedValues.ContainsKey(RouteValueKeys.AccountId.AuthorizationContextValueKey));
            Assert.AreEqual(456, fixtures.UnhashedValues[RouteValueKeys.AccountId.AuthorizationContextValueKey]);
        }
    }

    public class AccountIdInfoExtractorTestFixtures
    {
        public AccountIdInfoExtractorTestFixtures()
        {
            HashingServiceMock = new Mock<IHashingService>();
            HashingValuesMock = new Mock<IHashingValues>();
            ActionContext = new ActionContext
            {
                RouteData = new RouteData()
            };

            UnhashedValues = new Dictionary<string, long>();

            HashingValuesMock
                .Setup(hv => hv.Set(It.IsAny<string>(), It.IsAny<long>()))
                .Callback<string, long>((key, value) => UnhashedValues.Add(key, value));
        }

        public Mock<IHashingService> HashingServiceMock { get; }
        public IHashingService HashingService => HashingServiceMock.Object;

        public Mock<IHashingValues> HashingValuesMock { get; }
        public IHashingValues HashingValues => HashingValuesMock.Object;

        public ActionContext ActionContext { get; }

        public Dictionary<string,long> UnhashedValues { get; }

        public AccountIdInfoExtractorTestFixtures WithHashedAccountIdInRoute(string hash)
        {
            ActionContext.RouteData.Values.Add(RouteValueKeys.AccountId.RouteValueKey, hash);

            return this;
        }

        public AccountIdInfoExtractorTestFixtures WithRouteValue(string name, string hash)
        {
            ActionContext.RouteData.Values.Add(name, hash);

            return this;
        }

        public AccountIdInfoExtractorTestFixtures SetUnhashValue(string hash, long value)
        {
            HashingServiceMock.Setup(hs => hs.TryDecodeValue(hash, out value)).Returns(true);
            HashingServiceMock.Setup(hs => hs.DecodeValue(hash)).Returns(value);

            return this;
        }

        public AccountIdInfoExtractorTestFixtures RunValueExtractor()
        {
            var extractor = new AccountIdInfoExtractor(HashingService);

            extractor.BindModel(ActionContext, HashingValues);

            return this;
        }
    }
}
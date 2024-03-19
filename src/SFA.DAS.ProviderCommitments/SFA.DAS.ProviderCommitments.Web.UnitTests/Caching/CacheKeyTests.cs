using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderCommitments.Web.Caching;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Caching;

[TestFixture]
[Parallelizable]
public class CacheKeyTests
{
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2,Option3", true)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "2:1:1:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:2:1:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:3:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2,Option3,Option4", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option3,Option2,Option1", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:0:1:Option1,Option2,Option3", true)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "2:1:0:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:2:1:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:3:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option1,Option2,Option3,Option4", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option1,Option2", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option3,Option2,Option1", false)]
    public void GetHashCode_WhenComparingTwoObjects_ThenShouldReturnDifferentHashesAppropriately(string s1, string s2, bool expectToBeTheSame)
    {
        // Note: Should be the same for equal objects but might still be the same for completely different objects.
        //       The false cases have been selected because they are known to have different hashes. 
        var key1 = CreateCachedKeyFromString(s1);
        var key2 = CreateCachedKeyFromString(s2);

        CheckHash(key1, key2, expectToBeTheSame);
    }

    [TestCase("1:1:1:0:Option1,Option2,Option3")]
    public void GetHashCode_WhenComparingTheSameInstance_ThenShouldReturnTrue(string s1)
    {
        var key = CreateCachedKeyFromString(s1);

        CheckHash(key, key, true);
    }


    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2,Option3", true)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "2:1:1:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:2:1:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:3:0:Option1,Option2,Option3", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2,Option3,Option4", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option1,Option2", false)]
    [TestCase("1:1:1:0:Option1,Option2,Option3", "1:1:1:0:Option3,Option2,Option1", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:0:1:Option1,Option2,Option3", true)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "2:1:0:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:2:1:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:3:1:Option1,Option2,Option3", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option1,Option2,Option3,Option4", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option1,Option2", false)]
    [TestCase("1:1:0:1:Option1,Option2,Option3", "1:1:1:1:Option3,Option2,Option1", false)]
    [TestCase("1:1:2:7:Option1,Option2,Option3", "1:1:2:7:Option1,Option2,Option3", true)]
    public void Equals_WhenComparingTwoObjects_ThenShouldReturnCorrectValue(string s1, string s2, bool expectToBeTheSame)
    {
        // Note: Should be the same for equal objects but might still be the same for completely different objects.
        var key1 = CreateCachedKeyFromString(s1);
        var key2 = CreateCachedKeyFromString(s2);

        CheckEquality(key1, key2, expectToBeTheSame);
        CheckEqualityOperator(key1, key2, expectToBeTheSame);
    }

    private static void CheckHash(CacheKey k1, CacheKey k2, bool expectToBeTheSame)
    {
        var hash1 = k1.GetHashCode();
        var hash2 = k2.GetHashCode();

        Assert.That(hash1, expectToBeTheSame ? Is.EqualTo(hash2) : Is.Not.EqualTo(hash2));
    }

    private static void CheckEquality(CacheKey k1, CacheKey k2, bool expectToBeTheSame)
    {
        Assert.That(k1, expectToBeTheSame ? Is.EqualTo(k2) : Is.Not.EqualTo(k2));
    }

    private static void CheckEqualityOperator(CacheKey k1, CacheKey k2, bool expectToBeTheSame)
    {
        if (expectToBeTheSame)
        {
            Assert.That(k1, Is.EqualTo(k2));
            Assert.That(k1, Is.EqualTo(k2));
        }
        else
        {
            Assert.That(k1, Is.Not.EqualTo(k2));
            Assert.That(k1, Is.Not.EqualTo(k2));
        }
    }

    private static CacheKey CreateCachedKeyFromString(string s)
    {
        var parts = s.Split(new []{':'}, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 5)
        {
            throw new InvalidOperationException($"The test string should be in the format \"<party-type>:<party-id>:<cohort-id>:<option-1>,<option-2>...<option-n>\"");
        }

        var options = parts[4].Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);
        var apprenticeshipId = GetAsInt(parts, 3);
        var cohortId = GetAsInt(parts, 2);
        var party = (Party)GetAsInt(parts, 0);
        var partyId = GetAsInt(parts, 1);
        var authorizationContext = new AuthorizationContext();

        if (cohortId != 0)
        {
            authorizationContext.AddCommitmentPermissionValues(cohortId, party, partyId);
        }

        if (apprenticeshipId != 0)
        {
            authorizationContext.AddApprenticeshipPermissionValues(apprenticeshipId, party, partyId);
        }

        return new CacheKey(options, authorizationContext);
    }

    private static int GetAsInt(IReadOnlyList<string> options, int index)
    {
        if (!int.TryParse(options[index], out var result))
        {
            throw new InvalidOperationException($"The string {options[index]} should be an integer");
        }

        return result;
    }
}
using SFA.DAS.ProviderCommitments.Web.Helpers;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Helpers;


[TestFixture]
public class FlagExtensionsTests
{
    [Test]
    public void PipeDelimitedToFlags_ShouldReturnCorrectFlags()
    {
        string pipedelimitedlist = "One|Two|Four";
        int result = pipedelimitedlist.PipeDelimitedToFlags<TestEnum>();
        Assert.AreEqual(7, result);
    }

    [Test]
    public void ToFlags_ShouldReturnCorrectFlags()
    {
        List<string> list = new List<string> { "One", "Two", "Four" };
        int result = list.ToFlags<TestEnum>();
        Assert.AreEqual(7, result);
    }

    [Test]
    public void IsFlagSet_ShouldReturnTrue_WhenFlagIsSet()
    {
        int flags = 7;
        bool result = flags.IsFlagSet(TestEnum.One);
        Assert.IsTrue(result);
    }

    [Test]
    public void IsFlagSet_ShouldReturnFalse_WhenFlagIsNotSet()
    {
        int flags = 6;
        bool result = flags.IsFlagSet(TestEnum.One);
        Assert.IsFalse(result);
    }
}

public enum TestEnum
{
    None = 0,
    One = 1,
    Two = 2,
    Four = 4,
    Eight = 8
}
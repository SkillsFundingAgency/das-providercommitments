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
        ulong result = pipedelimitedlist.PipeDelimitedToFlags<TestEnum>();
        Assert.That(result, Is.EqualTo(7));
    }

    [Test]
    public void ToFlags_ShouldReturnCorrectFlags()
    {
        List<string> list = new List<string> { "One", "Two", "Four" };
        ulong result = list.ToFlags<TestEnum>();
        Assert.That(result, Is.EqualTo(7));
    }

    [Test]
    public void IsFlagSet_ShouldReturnTrue_WhenFlagIsSet()
    {
        ulong flags = 7;
        bool result = flags.IsFlagSet(TestEnum.One);
        Assert.IsTrue(result);
    }

    [Test]
    public void IsFlagSet_ShouldReturnFalse_WhenFlagIsNotSet()
    {
        ulong flags = 6;
        bool result = flags.IsFlagSet(TestEnum.One);
        Assert.IsFalse(result);
    }
}

public enum TestEnum : ulong
{
    None = 0,
    One = 1,
    Two = 2,
    Four = 4,
    Eight = 8
}
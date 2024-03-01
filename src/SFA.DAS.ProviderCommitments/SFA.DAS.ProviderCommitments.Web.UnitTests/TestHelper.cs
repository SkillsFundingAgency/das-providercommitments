using Newtonsoft.Json;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests;

public static class TestHelper
{
    public static T Clone<T>(T source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(serialized);
    }
}
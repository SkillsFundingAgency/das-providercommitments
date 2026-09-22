using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Responses;

public class GetApprenticeshipsResponseAlertsDeserializationTests
{
    [Test]
    public void Deserialize_ThenMapsIlrChangeInvalidFromApimName()
    {
        const string json = """
            {
              "apprenticeships": [
                {
                  "alerts": [ "IlrChangeInvalid" ]
                }
              ]
            }
            """;

        var result = JsonConvert.DeserializeObject<GetApprenticeshipsResponse>(json, CamelCaseSettings());

        result.Apprenticeships.Should().ContainSingle();
        result.Apprenticeships.Single().Alerts.Should().Equal(Alerts.IlrChangeInvalid);
    }

    private static JsonSerializerSettings CamelCaseSettings()
    {
        return new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}

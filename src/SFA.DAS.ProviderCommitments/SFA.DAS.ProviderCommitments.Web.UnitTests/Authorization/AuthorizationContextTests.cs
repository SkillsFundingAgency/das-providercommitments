using System;
using System.Collections.Generic;
using FluentAssertions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Authorization;

[TestFixture]
[Parallelizable]
public class AuthorizationContextTests
{
    [Test]
    public void Get_WhenKeyExists_ThenShouldReturnData()
    {
        var fixture = new AuthorizationContextTestsFixture();
        fixture.SetData();
        var result = fixture.GetData();
        result.Should().Be(fixture.Value);
    }

    [Test]
    public void Get_WhenKeyDoesNotExist_ThenShouldThrowException()
    {
        var fixture = new AuthorizationContextTestsFixture();
        Action result = () => fixture.GetData();
        result
            .Should()
            .Throw<KeyNotFoundException>()
            .WithMessage($"The key '{fixture.Key}' was not present in the authorization context");
    }

    [Test]
    public void TryGet_WhenKeyExists_ThenShouldReturnTrueAndValueShouldNotBeNull()
    {
        var fixture = new AuthorizationContextTestsFixture();
        fixture.SetData();
        var r = fixture.TryGetData();
        r.Should().BeTrue();
        fixture.ValueOut.Should().NotBeNull().And.Be(fixture.Value);
    }

    [Test]
    public void TryGet_WhenKeyDoesNotExist_ThenShouldReturnFalseAndValueShouldBeNull()
    {
        var fixture = new AuthorizationContextTestsFixture();
        var result = fixture.TryGetData();
        result.Should().BeFalse();
        fixture.ValueOut.Should().BeNull();
    }

    [Test]
    public void ToString_WhenKeysExist_ThenShouldReturnAuthorizedDescription()
    {
        var fixture = new AuthorizationContextTestsFixture();
        fixture.SetData(3);
        var result = fixture.AuthorizationContext.ToString();
        result.Should().Be("Foo_0: Bar_0, Foo_1: Bar_1, Foo_2: Bar_2");
    }

    [Test]
    public void ToString_WhenKeysDoNotExist_ThenShouldReturnUnauthorizedDescription()
    {
        var fixture = new AuthorizationContextTestsFixture();
        var result = fixture.AuthorizationContext.ToString();
        result.Should().Be("None");
    }
}

public class AuthorizationContextTestsFixture
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string ValueOut { get; set; }
    public IAuthorizationContext AuthorizationContext { get; set; }

    public AuthorizationContextTestsFixture()
    {
        Key = "Foo";
        Value = "Bar";
        AuthorizationContext = new AuthorizationContext();
    }

    public string GetData()
    {
        return AuthorizationContext.Get<string>(Key);
    }

    public AuthorizationContextTestsFixture SetData(int count = 1)
    {
        if (count == 1)
        {
            AuthorizationContext.Set(Key, Value);
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                AuthorizationContext.Set($"{Key}_{i}", $"{Value}_{i}");
            }
        }

        return this;
    }

    public bool TryGetData()
    {
        var exists = AuthorizationContext.TryGet(Key, out string value);

        ValueOut = value;

        return exists;
    }
}
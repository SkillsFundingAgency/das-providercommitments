﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Error;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ErrorControllerTests
{
    [TestFixture]
    public class ErrorControllerTest
    {
        public Mock<IConfiguration> Configuration;
        public bool UseDfESignIn;
        public ErrorController Sut { get; set; }

        [Test]
        [TestCase("test", "https://test-services.signin.education.gov.uk/organisations")]
        [TestCase("pp", "https://test-services.signin.education.gov.uk/organisations")]
        [TestCase("local", "https://test-services.signin.education.gov.uk/organisations")]
        [TestCase("prd", "https://services.signin.education.gov.uk/organisations")]
        public void Then_The_Page_Returns_HelpLink(string env, string helpLink)
        {
            var fixture = new Fixture();

            UseDfESignIn = fixture.Create<bool>();

            Configuration = new Mock<IConfiguration>();
            Sut = new ErrorController(Configuration.Object);
            
            Configuration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
            Configuration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(UseDfESignIn));


            var result = (ViewResult)Sut.Error(403);
            result.ViewName.Should().Be("403");

            Assert.That(result, Is.Not.Null);
            var actualModel = result?.Model as Error403ViewModel;
            Assert.That(actualModel?.HelpPageLink, Is.EqualTo(helpLink));
            Assert.AreEqual(actualModel?.UseDfESignIn, UseDfESignIn);
        }
    }
}

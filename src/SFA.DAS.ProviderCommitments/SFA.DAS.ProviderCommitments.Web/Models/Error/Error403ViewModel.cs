﻿using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Error
{
    public class Error403ViewModel
    {
        private readonly string _integrationUrlPart = string.Empty;

        public Error403ViewModel(string environment)
        {
            if (!environment.Equals("prd", StringComparison.CurrentCultureIgnoreCase))
            {
                _integrationUrlPart = "test-";
            }
        }

        public bool UseDfESignIn { get; set; }
        public string HelpPageLink => $"https://{_integrationUrlPart}services.signin.education.gov.uk/organisations";
    }
}

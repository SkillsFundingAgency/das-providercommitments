﻿namespace SFA.DAS.ProviderCommitments.Configuration
{
    public static class ProviderCommitmentsConfigurationKeys
    {
        public const string Encoding = "SFA.DAS.Encoding";
        public const string ProviderCommitments = "SFA.DAS.ProviderCommitments";
        public static string AuthenticationSettings = $"{ProviderCommitments}:AuthenticationSettings";
        public static string ApprenticeshipInfoServiceConfiguration = $"{ProviderCommitments}:ApprenticeshipInfoServiceConfiguration";
        public static string CommitmentsClientApiConfiguration = $"{ProviderCommitments}:CommitmentsClientApi";
        public static string FeaturesConfiguration = $"{ProviderCommitments}:Features";
        public static string ZenDeskConfiguration = $"{ProviderCommitments}:ZenDesk";
    }
}

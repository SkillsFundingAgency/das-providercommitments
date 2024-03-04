namespace SFA.DAS.ProviderCommitments.Web.Controllers;

public static class ControllerConstants
{
    public static class ApprenticeController
    {
        public const string Name = "Apprentice";

        public static class Actions
        {
            public const string Index = nameof(Index);
        }
    }

    public static class CohortController
    {
        public const string Name = "Cohort";

        public static class Actions
        {
            public const string Details = nameof(Details);
            public const string Review = nameof(Review);
        }
    }

    public static class OverlappingTrainingDateRequestController
    {
        public const string Name = "OverlappingTrainingDateRequest";

        public static class Actions
        {
            public const string EmployerNotified = nameof(EmployerNotified);
            public const string ChangeOfEmployerNotified = nameof(ChangeOfEmployerNotified);
            public const string OverlapOptionsForChangeEmployer = nameof(OverlapOptionsForChangeEmployer);
        }
    }
}
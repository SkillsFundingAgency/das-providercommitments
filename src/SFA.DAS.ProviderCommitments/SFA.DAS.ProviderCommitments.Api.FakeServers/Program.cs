using System;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            PasAccountsApiBuilder.Create(44378)
                .WithAgreementStatusSet()
                .Build();

            CommitmentsOuterApiBuilder.Create(10234)
                .WithCourseDeliveryModels()
                .Build();

            Console.WriteLine("PAS Accounts running on port 44378");
            Console.WriteLine("Approvals Outer API running on port 10234");
            Console.WriteLine("Course Games Developer (650) has a Single DeliveryModel, all other course have multiple");
            Console.WriteLine("Press any key to stop the APIs server");
            Console.ReadKey();
        }
    }
}
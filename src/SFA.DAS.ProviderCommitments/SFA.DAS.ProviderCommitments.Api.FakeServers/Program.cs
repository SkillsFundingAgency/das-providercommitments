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

            Console.WriteLine("Press any key to stop the APIs server");
            Console.ReadKey();
        }
    }
}
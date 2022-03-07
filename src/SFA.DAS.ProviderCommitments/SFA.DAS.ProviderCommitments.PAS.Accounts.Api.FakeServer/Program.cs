using System;

namespace SFA.DAS.ProviderCommitments.PAS.Accounts.Api.FakeServer
{
    public static class Program
    {
        public static void Main(string[] args)
        { 
            PasAccountsApiBuilder.Create(44378)
                .WithAgreementStatusSet()
                .Build();

            Console.WriteLine("Press any key to stop the PAS Accounts API server");
            Console.ReadKey();
        }
    }
}

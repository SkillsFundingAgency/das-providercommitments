using System;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers;

public static class Program
{
    public static void Main(string[] args)
    {

        if (args.Contains("--h"))
        {
            Console.WriteLine("examples:");
            Console.WriteLine("SFA.DAS.ProviderCommitments.Api.FakeServers --h                 <-- shows this page");
            Console.WriteLine("To exclude multiple APIs use any combination of !PasAccounts !OuterApi as input params (the parameters are case insensitive)");

            return;
        }


        if (!args.Contains("!PasAccounts", StringComparer.CurrentCultureIgnoreCase))
        {
            PasAccountsApiBuilder.Create(44378)
                .WithAgreementStatusSet()
                .Build();
        }

        if (!args.Contains("!OuterApi", StringComparer.CurrentCultureIgnoreCase))
        {
            CommitmentsOuterApiBuilder.Create(10234)
                .WithCourseDeliveryModels()
                .WithBulkUpload()
                .Build();
        }

        Console.WriteLine("Course Games Developer (650) has a Single DeliveryModel, all other course have multiple");
        Console.WriteLine("Press any key to stop the APIs server");
        Console.ReadKey();
    }
}
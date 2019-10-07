using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Services;
using SFA.DAS.EAS.Account.Api.Client;
using StructureMap;

namespace SFA.DAS.Commitments.Shared.DependencyInjection
{
    public class EmployerAccountsRegistry : Registry
    {
        public EmployerAccountsRegistry()
        {
            For<IEmployerAgreementService>().Use<EmployerAgreementService>().Singleton();
            For<IAccountApiClient>().Use(c => new AccountApiClient(c.GetInstance<AccountApiConfiguration>()));
        }
    }
}
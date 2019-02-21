using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IApprenticeshipInfoService
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(long ukPrn);
    }
}
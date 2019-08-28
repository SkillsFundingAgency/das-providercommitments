using System.Threading.Tasks;

namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface IModelMapper
    {
        Task<T> Map<T>(object source) where T : class;
    }
}

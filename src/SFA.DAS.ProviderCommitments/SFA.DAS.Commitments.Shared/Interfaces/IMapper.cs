using System.Threading.Tasks;

namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface IMapper<in TFrom, TTo> where TFrom : class where TTo : class
    {
        Task<TTo> Map(TFrom source);
    }
}
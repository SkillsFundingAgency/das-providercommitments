
namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public interface IMapper<in TFrom, out TTo> where TFrom : class where TTo : class
    {
        TTo Map(TFrom source);
    }
}
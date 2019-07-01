namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface IMapper<in TFrom, out TTo> where TFrom : class where TTo : class
    {
        TTo Map(TFrom source);
    }
}
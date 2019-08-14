namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface IModelMapper
    {
        T Map<T>(object source) where T : class;
    }
}

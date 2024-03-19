namespace SFA.DAS.ProviderCommitments.Interfaces;

public interface ICommitmentsAuthorisationHandler
{
    Task<bool> CanAccessCohort();
    Task<bool> CanAccessApprenticeship();
}
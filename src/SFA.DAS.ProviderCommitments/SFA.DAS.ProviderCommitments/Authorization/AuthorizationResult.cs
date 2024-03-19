using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Authorization;

public class AuthorizationResult
{
    public bool IsAuthorized => _errors.Count == 0;
    public IEnumerable<AuthorizationError> Errors => _errors;

    private readonly List<AuthorizationError> _errors = new();

    public AuthorizationResult()
    {
    }

    public AuthorizationResult(AuthorizationError error)
    {
        _errors.Add(error);
    }

    public AuthorizationResult(IEnumerable<AuthorizationError> errors)
    {
        _errors.AddRange(errors);
    }

    public AuthorizationResult AddError(AuthorizationError error)
    {
        _errors.Add(error);

        return this;
    }

    public bool HasError<T>() where T : AuthorizationError
    {
        return _errors.OfType<T>().Any();
    }

    public override string ToString()
    {
        return $"{nameof(IsAuthorized)}: {IsAuthorized}, {nameof(Errors)}: {(_errors.Count > 0 ? string.Join(", ", _errors.Select(e => e.Message)) : "None")}";
    }
}
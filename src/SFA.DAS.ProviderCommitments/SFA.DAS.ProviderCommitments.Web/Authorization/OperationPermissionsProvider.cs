using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public interface IOperationPermissionsProvider
{
    void Save(OperationPermission operationPermission);
    bool TryGetPermission(long? accountLegalEntityId, Operation operation, out bool result);
}

public class OperationPermissionsProvider(IHttpContextAccessor httpContextAccessor) : IOperationPermissionsProvider
{
    public bool TryGetPermission(long? accountLegalEntityId, Operation operation, out bool result)
    {
        result = false;

        var permissions = GetPermissionsFromClaims();

        if (permissions == null || !permissions.Any())
        {
            return false;
        }

        if (permissions.Exists(x => x.AccountLegalEntityId == accountLegalEntityId && x.Operation == operation))
        {
            result = permissions.Single(x => x.AccountLegalEntityId == accountLegalEntityId && x.Operation == operation).HasPermission;
            return true;
        }

        return false;
    }
    
    public void Save(OperationPermission operationPermission)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (!user.HasClaim(x => x.Type.Equals(ProviderClaims.OperationPermissions)))
        {
            AddClaim(operationPermission);
        }
        else
        {
            var operationPermissions = GetPermissionsFromClaims();
            operationPermissions.Add(operationPermission);

            UpdateClaim(operationPermissions);
        }
    }
    
    private List<OperationPermission> GetPermissionsFromClaims()
    {
        var user = httpContextAccessor.HttpContext?.User;
        
        return JsonConvert.DeserializeObject<List<OperationPermission>>(user.GetClaimValue(ProviderClaims.OperationPermissions));
    }
    
    private void AddClaim(OperationPermission operationPermission)
    {
        httpContextAccessor.HttpContext?.User.Identities.First()
            .AddClaim(new Claim(ProviderClaims.OperationPermissions,
                JsonConvert.SerializeObject(new List<OperationPermission> { operationPermission }), JsonClaimValueTypes.Json)
            );
    }

    private void UpdateClaim(IEnumerable<OperationPermission> permissions)
    {
        var claimsIdentity = httpContextAccessor.HttpContext?.User.Identities.First();

        claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(x => x.Type.Equals(ProviderClaims.OperationPermissions)));

        claimsIdentity.AddClaim(new Claim(ProviderClaims.OperationPermissions, JsonConvert.SerializeObject(permissions), JsonClaimValueTypes.Json));
    }
}
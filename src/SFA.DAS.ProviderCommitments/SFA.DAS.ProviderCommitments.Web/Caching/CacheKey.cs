using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Caching;

public class CacheKey
{
    public IReadOnlyCollection<string> Options { get; }
    public long CohortId { get; }
    public long ApprenticeshipId { get; }
    public Party Party { get; }
    public long PartyId { get; }

    private readonly int _hashCode;

    public CacheKey(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        Options = options;
            
        if (authorizationContext.TryGetPermissionValues(out var cohortId, out var apprenticeshipId, out var party, out var partyId))
        {
            CohortId = cohortId;
            ApprenticeshipId = apprenticeshipId;
            Party = party;
            PartyId = partyId;
        }
            
        _hashCode = CalculateHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is CacheKey other &&
               other.Options.IsSameAs(Options) &&
               other.Party == Party &&
               other.PartyId == PartyId &&
               other.CohortId == CohortId &&
               other.ApprenticeshipId == ApprenticeshipId;
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public static bool operator == (CacheKey lhs, CacheKey rhs)
    {
        if (ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
        {
            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator != (CacheKey lhs, CacheKey rhs)
    {
        return !(lhs == rhs);
    }

    private int CalculateHashCode()
    {
        unchecked
        {
            var hash = (int)2166136261;
                
            hash = (hash * 16777619) ^ Party.GetHashCode();
            hash = (hash * 16777619) ^ PartyId.GetHashCode();
            hash = (hash * 16777619) ^ CohortId.GetHashCode();
            hash = (hash * 16777619) ^ ApprenticeshipId.GetHashCode();
                
            if (Options != null)
            {
                foreach (var option in Options)
                {
                    hash = (hash * 16777619) ^ option.GetHashCode();
                }
            }

            return hash;
        }
    }
}
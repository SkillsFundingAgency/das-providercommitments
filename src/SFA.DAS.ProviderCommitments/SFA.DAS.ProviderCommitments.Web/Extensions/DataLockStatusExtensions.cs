using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class DataLockStatusExtensions
    {
        public static DetailsViewModel.DataLockSummaryStatus GetDataLockSummaryStatus(this IReadOnlyCollection<GetDataLocksResponse.DataLock> dataLocks)
        {
            DetailsViewModel.DataLockSummaryStatus dataLockStatus = DetailsViewModel.DataLockSummaryStatus.None;
            if (dataLocks.Any(x => x.TriageStatus != TriageStatus.Unknown && IsUnresolvedDataLock(x)))
            {
                dataLockStatus = DetailsViewModel.DataLockSummaryStatus.AwaitingTriage;
            }
            else if (dataLocks.Any(x => x.TriageStatus == TriageStatus.Unknown && IsUnresolvedDataLock(x)))
            {
                dataLockStatus = DetailsViewModel.DataLockSummaryStatus.HasUnresolvedDataLocks;
            }

            return dataLockStatus;
        }

        private static bool IsUnresolvedDataLock(GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.DataLockStatus != Status.Pass && !dataLock.IsResolved;
        }
    }
}
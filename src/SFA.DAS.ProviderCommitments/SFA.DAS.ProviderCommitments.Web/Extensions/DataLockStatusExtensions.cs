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

        public static bool IsUnresolvedError(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.DataLockStatus != Status.Pass && !dataLock.IsResolved;
        }

        public static bool IsCourse(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.HasCourseDataLock() && !dataLock.HasPrice();
        }

        public static bool IsPrice(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.ErrorCode == DataLockErrorCode.Dlock07;
        }

        public static bool HasPrice(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.ErrorCode.HasFlag(DataLockErrorCode.Dlock07);
        }

        public static bool IsCourseAndPrice(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.HasCourseDataLock() && dataLock.HasPrice();
        }

        public static bool HasCourseDataLock(this GetDataLocksResponse.DataLock dataLock)
        {
            var result = dataLock.ErrorCode.HasFlag(DataLockErrorCode.Dlock03) ||
                         dataLock.ErrorCode.HasFlag(DataLockErrorCode.Dlock04) ||
                         dataLock.ErrorCode.HasFlag(DataLockErrorCode.Dlock05) ||
                         dataLock.ErrorCode.HasFlag(DataLockErrorCode.Dlock06);
            return result;
        }

        public static bool IsCourseOrPrice(this GetDataLocksResponse.DataLock dataLock)
        {
            return dataLock.IsCourse() || dataLock.IsPrice();
        }
        
    }
}
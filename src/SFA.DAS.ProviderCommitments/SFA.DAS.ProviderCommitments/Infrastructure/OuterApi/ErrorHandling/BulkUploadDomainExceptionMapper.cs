using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling
{
    public static class BulkUploadDomainExceptionMapper
    {
        public static List<BulkUploadValidationError> ToBulkUploadValidationErrors(CommitmentsApiModelException exception)
        {
            var errors = (exception?.Errors ?? [])
                .Select(e => new Error(e.Field ?? string.Empty, e.Message))
                .ToList();

            return
            [
                new BulkUploadValidationError(0, string.Empty, string.Empty, string.Empty, errors)
            ];
        }

        public static string ToFieldMessageLog(CommitmentsApiModelException exception)
        {
            var errors = exception?.Errors;
            if (errors == null || errors.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("; ", errors.Select(e => $"{e.Field}: {e.Message}"));
        }
    }
}

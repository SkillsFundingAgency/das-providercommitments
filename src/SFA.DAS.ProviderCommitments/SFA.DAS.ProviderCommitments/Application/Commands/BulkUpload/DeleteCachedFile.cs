using MediatR;
using System;

namespace SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload
{
    public class DeleteCachedFileCommand : IRequest
    {
        public Guid CachedRequestId { get; set; }
    }
}

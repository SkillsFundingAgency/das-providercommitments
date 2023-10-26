﻿using MediatR;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload
{
    public class DeleteCachedFileHandler : IRequestHandler<DeleteCachedFileCommand>
    {
        private readonly ICacheService _cacheService;

        public DeleteCachedFileHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteCachedFileCommand request, CancellationToken cancellationToken)
        {
            await _cacheService.ClearCache(request.CachedRequestId.ToString(), nameof(DeleteCachedFileHandler));
        }
    }
}

﻿using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public interface IBlobFileTransferClient
    {
        Task DeleteFile(string path);
        Task<string> DownloadFile(string path);
        Task UploadFile(string fileContents, string path);
    }
}
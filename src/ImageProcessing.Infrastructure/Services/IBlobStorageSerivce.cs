// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Storage.Blobs;

namespace ImageProcessing.Infrastructure.Services
{
    public interface IBlobStorageService
    {
        BlobClient GetBlobClient(string blobName);
        Task<Stream> DownloadBlobAsStream(string subFolder, string blobName);
        Task<Uri> UploadBlobAsync(Stream content, string subFolder, string blobName);
    }
}

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Storage.Blobs;

namespace ImageProcessing.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(string connectionString, string blobContainerName)
        {
            _serviceClient = new BlobServiceClient(connectionString);
            _containerClient = _serviceClient.GetBlobContainerClient(blobContainerName);
        }

        public BlobClient GetBlobClient(string blobName)
        {
            var client = _containerClient.GetBlobClient(blobName);
            return client;
        }

        public async Task<Stream> DownloadBlobAsStream(string subFolder, string blobName)
        {
            var blobFullPath = $"{subFolder}/{blobName}";
            var blobClient = _containerClient.GetBlobClient(blobFullPath);
            var blobContent = await blobClient.OpenReadAsync();
            return blobContent;
        }

        public async Task<Uri> UploadBlobAsync(Stream content, string subFolder, string blobName)
        {
            var blobFullName = $"{subFolder}/{blobName}";
            var blobClient = _containerClient.GetBlobClient(blobFullName);

            using (content)
            {
                await _containerClient.UploadBlobAsync(blobFullName, content);
            }

            return blobClient.Uri;
        }
    }
}

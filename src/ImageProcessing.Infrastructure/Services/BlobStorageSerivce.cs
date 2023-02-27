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

        /// <summary>
        /// Get client for specific blob
        /// </summary>
        /// <param name="blobName">Full path to blob</param>
        /// <returns>Instance of client to work with blob</returns>
        public BlobClient GetBlobClient(string blobName)
        {
            var client = _containerClient.GetBlobClient(blobName);
            return client;
        }

        /// <summary>
        /// Download blob from storage
        /// </summary>
        /// <param name="subFolder">Folder where blob is stored</param>
        /// <param name="blobName">Name of blob to download</param>
        /// <returns>Stream with blob content</returns>
        public async Task<Stream> DownloadBlobAsStream(string subFolder, string blobName)
        {
            var blobFullPath = $"{subFolder}/{blobName}";
            var blobClient = _containerClient.GetBlobClient(blobFullPath);
            var blobContent = await blobClient.OpenReadAsync();
            return blobContent;
        }

        /// <summary>
        /// Upload blob to storage
        /// </summary>
        /// <param name="content">Stream with content to upload</param>
        /// <param name="subFolder">Folder where to upload blob</param>
        /// <param name="blobName">Blob name</param>
        /// <returns></returns>
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

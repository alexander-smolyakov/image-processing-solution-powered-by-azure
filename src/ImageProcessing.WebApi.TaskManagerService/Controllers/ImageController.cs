﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Storage.Blobs;
using ImageProcessing.Core.Tools;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessing.WebApi.TaskManagerService.Controllers
{
    [ApiController]
    public class ImageController : ServiceControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly BlobServiceClient _blobService;
        private readonly ICosmosDbService _cosmosDbService;

        public ImageController(ICosmosDbService cosmosDbService, BlobServiceClient blobService, ILogger<ImageController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobService = blobService;
            _logger = logger;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var taskId = Guid.NewGuid();

            var blobName = $"{taskId.ToString()}/{file.FileName}";
            var containerClient = _blobService.GetBlobContainerClient("image-storage");
            var blobClient = containerClient.GetBlobClient(blobName);
            await containerClient.UploadBlobAsync(blobName, file.OpenReadStream());

            var imageUrl = blobClient.Uri;

            var task = ProcessingTaskTools.GenerateProcessingTask(taskId: taskId, fileName: file.Name, imageUrl: imageUrl);
            await _cosmosDbService.AddItemAsync(task);

            return Ok(task);
        }
    }
}

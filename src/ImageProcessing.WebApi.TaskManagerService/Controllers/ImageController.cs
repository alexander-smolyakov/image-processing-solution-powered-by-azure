// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Storage.Blobs;
using ImageProcessing.Core.Entities;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = ImageProcessing.Core.Entities.TaskStatus;

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

            await _blobService.CreateBlobContainerAsync(taskId.ToString());
            var containerClient = _blobService.GetBlobContainerClient(taskId.ToString());
            var blobClient = containerClient.GetBlobClient(file.FileName);
            await containerClient.UploadBlobAsync(file.FileName, file.OpenReadStream());

            var imageUrl = blobClient.Uri;

            var task = new ProcessingTask()
            {
                Id = taskId,
                Status = TaskStatus.Created,
                StatusAsString = TaskStatus.Created.ToString(),
                FileName = file.FileName,
                OriginalImageUrl = imageUrl
            };

            await _cosmosDbService.AddItemAsync(task);

            return Ok(task);
        }
    }
}

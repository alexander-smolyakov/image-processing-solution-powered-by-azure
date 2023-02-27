// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Tools;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessing.WebApi.TaskManagerService.Controllers
{
    [ApiController]
    public class ImageController : ServiceControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IBlobStorageService _blobService;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IBusTopicService _busTopicService;

        public ImageController(ICosmosDbService cosmosDbService, IBlobStorageService blobService, IBusTopicService busTopicService, ILogger<ImageController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobService = blobService;
            _busTopicService = busTopicService;
            _logger = logger;
        }

        [HttpPost]
        [Route("upload")]
        /// <summary>
        /// Upload image to Azure BlobStore
        /// </summary>
        /// <remarks>
        /// Upload image to Azure BlobStore and create task to process image
        /// </remarks>
        /// <param name="file">File to upload</param>s
        /// <returns>Task ID and path to uploaded image</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var taskId = Guid.NewGuid();
            var fileName = file.FileName;

            if (ImageTool.IsSupportedImageFormat(fileName) == false)
            {
                return BadRequest(new { error = "Not supported file format" });
            }

            _logger.LogInformation($"Upload {fileName} to Azure Blob Storage");
            var blobUrl = await _blobService.UploadBlobAsync(
                content: file.OpenReadStream(),
                subFolder: taskId.ToString(),
                blobName: fileName
           );

            _logger.LogInformation($"Generate task with id: {taskId}");
            var task = ProcessingTaskTools.GenerateProcessingTask(
                taskId: taskId,
                fileName: fileName,
                imageUrl: blobUrl
            );

            _logger.LogInformation($"Add task {task.Id} to Cosmos DB");
            await _cosmosDbService.AddItemAsync(task);

            _logger.LogInformation($"Send topic to service bus");
            await _busTopicService.SendMessagesAsync(task);

            return Ok(new { TaskId = task.Id, OriginalImageUrl = task.OriginalImageUrl });
        }
    }
}

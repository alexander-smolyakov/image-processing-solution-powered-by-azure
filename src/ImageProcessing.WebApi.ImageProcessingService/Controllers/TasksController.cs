// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Tools;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using TaskStatus = ImageProcessing.Core.Entities.TaskStatus;

namespace ImageProcessing.WebApi.ImageProcessingService.Controllers
{
    public class TasksController : ServiceControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly IBlobStorageService _blobService;
        private readonly ICosmosDbService _cosmosDbService;

        public TasksController(ICosmosDbService cosmosDbService, IBlobStorageService blobService, ILogger<TasksController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobService = blobService;
            _logger = logger;
        }

        [HttpPost]
        [Route("process")]
        public async Task<IActionResult> ProcessTask(Guid taskId)
        {
            _logger.LogInformation($"Get task from Cosmos DB with id: {taskId}");
            var task = await _cosmosDbService.GetItemAsync(taskId.ToString());

            if (task is null)
            {
                return NotFound($"Task with ID: {taskId} not found");
            }

            if (task.Status == TaskStatus.Done)
            {
                return Ok(new { processedImageUrl = task.ProcessedImageUrl });
            }

            _logger.LogInformation($"Set task {taskId} status to InProgress");
            task = ProcessingTaskTools.UpdateTaskStatus(task, TaskStatus.InProgress);
            await _cosmosDbService.UpdateItemAsync(taskId.ToString(), task);

            _logger.LogInformation($"Download {task.FileName} from blob storage");
            var blobContent = await _blobService.DownloadBlobAsStream(subFolder: taskId.ToString(), blobName: task.FileName);

            _logger.LogInformation($"Rotate {task.FileName}");
            (Image content, IImageEncoder encoder) processedImage = await ImageTool.RotateImageAsync(blobContent);

            _logger.LogInformation($"Upload rotated {task.FileName} to blob storage");
            using (var memoryStream = new MemoryStream())
            {
                await processedImage.content.SaveAsync(stream: memoryStream, encoder: processedImage.encoder);
                memoryStream.Position = 0;
                task.ProcessedImageUrl = await _blobService.UploadBlobAsync(content: memoryStream, subFolder: taskId.ToString(), blobName: $"processed_{task.FileName}");
            }

            _logger.LogInformation($"Set task {taskId} status to Done");
            task = ProcessingTaskTools.UpdateTaskStatus(task, TaskStatus.Done);
            await _cosmosDbService.UpdateItemAsync(taskId.ToString(), task);

            return Ok(new { processedImageUrl = task.ProcessedImageUrl });
        }
    }
}

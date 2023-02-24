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
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var taskId = Guid.NewGuid();
            var fileName = file.FileName;

            var blobUrl = await _blobService.UploadBlobAsync(
                content: file.OpenReadStream(),
                subFolder: taskId.ToString(),
                blobName: fileName
           );

            var task = ProcessingTaskTools.GenerateProcessingTask(
                taskId: taskId,
                fileName: fileName,
                imageUrl: blobUrl
            );

            await _cosmosDbService.AddItemAsync(task);

            await _busTopicService.SendMessagesAsync(task);

            return Ok(task);
        }
    }
}

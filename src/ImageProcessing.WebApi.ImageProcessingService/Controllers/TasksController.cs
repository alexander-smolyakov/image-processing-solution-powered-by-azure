// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Storage.Blobs;
using ImageProcessing.Core.Entities;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using TaskStatus = ImageProcessing.Core.Entities.TaskStatus;

namespace ImageProcessing.WebApi.ImageProcessingService.Controllers
{
    public class TasksController : ServiceControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly BlobServiceClient _blobService;
        private readonly ICosmosDbService _cosmosDbService;

        public TasksController(ICosmosDbService cosmosDbService, BlobServiceClient blobService, ILogger<TasksController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobService = blobService;
            _logger = logger;
        }

        [HttpPost]
        [Route("process")]
        public async Task<IActionResult> UploadImage(Guid taskId)
        {
            var task = await _cosmosDbService.GetItemAsync(taskId.ToString());

            task.Status = TaskStatus.InProgress;
            task.StatusAsString = TaskStatus.InProgress.ToString();
            task.UpdatedAt = DateTime.UtcNow;

            await _cosmosDbService.UpdateItemAsync(taskId.ToString(), task);

            var containerClient = _blobService.GetBlobContainerClient(taskId.ToString());
            var blobClient = containerClient.GetBlobClient(task.FileName);

            var stream = await blobClient.OpenReadAsync();
            (Image myImage, IImageFormat Format) imf = await Image.LoadWithFormatAsync(stream);
            var ifm = Configuration.Default.ImageFormatsManager;
            var format = ifm.FindFormatByMimeType(imf.Format.DefaultMimeType);
            var encoder = ifm.FindEncoder(format);

            blobClient = containerClient.GetBlobClient($"rotated_{task.FileName}");

            imf.myImage.Mutate(x => x.RotateFlip(rotateMode: RotateMode.Rotate180, flipMode: FlipMode.None));

            using (var ms = new MemoryStream())
            {
                await imf.myImage.SaveAsync(stream: ms, encoder: encoder);
                ms.Position = 0;
                await blobClient.UploadAsync(ms);
            }

            var imageUrl = blobClient.Uri;

            task.Status = TaskStatus.Done;
            task.StatusAsString = TaskStatus.Done.ToString();
            task.ProcessedImageUrl = imageUrl;
            task.UpdatedAt = DateTime.UtcNow;

            await _cosmosDbService.UpdateItemAsync(taskId.ToString(), task);

            return Ok(task);
        }
    }
}

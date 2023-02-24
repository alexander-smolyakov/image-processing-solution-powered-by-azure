// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Entities;
using ImageProcessing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessing.WebApi.TaskManagerService.Controllers
{
    [ApiController]
    public class TasksController : ServiceControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly ICosmosDbService _cosmosDbService;

        public TasksController(ICosmosDbService cosmosDbService, ILogger<TasksController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ProcessingTask>> GetAllTasks()
        {
            var tasks = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return tasks;
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetTaskByID(Guid id)
        {
            var task = await _cosmosDbService.GetItemAsync(id.ToString());
            if (task is null)
            {
                return BadRequest($"Task doesn't exist {id}");
            }

            return Ok(task);
        }
    }
}

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

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
        /// <summary>
        /// Get all tasks from Cosmos DB
        /// </summary>
        /// <returns>Collection of tasks</returns>
        /// <response code="200">OK</response>
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("Request all tasks from Cosmos DB");
            var tasks = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return Ok(tasks);
        }

        [HttpGet("{id:Guid}")]
        /// <summary>
        /// Get tasks from Cosmos DB by specifing id
        /// </summary>
        /// <returns>Collection of tasks</returns>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        public async Task<IActionResult> GetTaskByID(Guid id)
        {
            _logger.LogInformation($"Request task by id: {id}");
            var task = await _cosmosDbService.GetItemAsync(id.ToString());
            if (task is null)
            {
                return NotFound($"Task with {id} not found");
            }

            return Ok(task);
        }
    }
}

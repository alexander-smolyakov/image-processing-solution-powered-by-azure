// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Entities;
using TaskStatus = ImageProcessing.Core.Entities.TaskStatus;

namespace ImageProcessing.Core.Tools
{
    public static class ProcessingTaskTools
    {
        /// <summary>
        /// Generate new processing task
        /// </summary>
        /// <param name="taskId">Task indemnificator</param>
        /// <param name="fileName">File attached to processing task</param>
        /// <param name="imageUrl">Uri to uploaded file</param>
        /// <returns>Instance of processing task</returns>
        public static ProcessingTask GenerateProcessingTask(Guid taskId, string fileName, Uri imageUrl)
        {
            return new ProcessingTask()
            {
                Id = taskId,
                Status = TaskStatus.Created,
                StatusAsString = TaskStatus.Created.ToString(),
                FileName = fileName,
                OriginalImageUrl = imageUrl
            };
        }

        /// <summary>
        /// Update processing task status
        /// </summary>
        /// <param name="task">Instance of the task to update</param>
        /// <param name="taskStatus">File attached to processing task</param>
        /// <returns>Updated instance of processing task</returns>
        public static ProcessingTask UpdateTaskStatus(ProcessingTask task, TaskStatus taskStatus)
        {
            task.Status = taskStatus;
            task.StatusAsString = taskStatus.ToString();
            task.UpdatedAt = DateTime.UtcNow;
            return task;
        }
    }
}

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Entities;
using TaskStatus = ImageProcessing.Core.Entities.TaskStatus;

namespace ImageProcessing.Core.Tools
{
    public static class ProcessingTaskTools
    {
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
        public static ProcessingTask UpdateTaskStatus(ProcessingTask task, TaskStatus taskStatus)
        {
            task.Status = taskStatus;
            task.StatusAsString = taskStatus.ToString();
            task.UpdatedAt = DateTime.UtcNow;
            return task;
        }
    }
}

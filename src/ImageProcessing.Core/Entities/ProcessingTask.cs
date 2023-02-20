// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace ImageProcessing.Core.Entities
{
    public class ProcessingTask : BaseEntity
    {
        public TaskStatus Status { get; set; }
        public string? StatusAsString { get; set; }
        public string FileName { get; set; } = "file";
        public Uri OriginalImageUrl { get; set; } = new Uri("about:blank");
        public Uri ProcessedImageUrl { get; set; } = new Uri("about:blank");
    }
}

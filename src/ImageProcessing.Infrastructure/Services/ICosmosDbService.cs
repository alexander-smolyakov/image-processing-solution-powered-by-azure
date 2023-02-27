// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Entities;
namespace ImageProcessing.Infrastructure.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<ProcessingTask>> GetItemsAsync(string query);
        Task<ProcessingTask?> GetItemAsync(string id);
        Task AddItemAsync(ProcessingTask item);
        Task UpdateItemAsync(string id, ProcessingTask item);
        Task DeleteItemAsync(string id);
    }
}

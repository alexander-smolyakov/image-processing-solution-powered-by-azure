// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using ImageProcessing.Core.Entities;
using Microsoft.Azure.Cosmos;

namespace ImageProcessing.Infrastructure.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        /// <summary>
        /// Insert item to DB
        /// </summary>
        /// <param name="item">Instance of item to add</param>
        /// <returns></returns>
        public async Task AddItemAsync(ProcessingTask item)
        {
            await _container.CreateItemAsync<ProcessingTask>(item, new PartitionKey(item.Id.ToString()));
        }

        /// <summary>
        /// Delete item from DB
        /// </summary>
        /// <param name="id">Id of item to delete</param>
        /// <returns></returns>
        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<ProcessingTask>(id, new PartitionKey(id));
        }

        /// <summary>
        /// Get item from DB by id
        /// </summary>
        /// <param name="id">Id of item to get</param>
        /// <returns>Instance of task</returns>
        public async Task<ProcessingTask?> GetItemAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<ProcessingTask>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Get items collection by query 
        /// </summary>
        /// <param name="queryString">Qeury to execute against DB</param>
        /// <returns></returns>
        public async Task<IEnumerable<ProcessingTask>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<ProcessingTask>(new QueryDefinition(queryString));
            var results = new List<ProcessingTask>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        /// <summary>
        /// Upadte item in DB
        /// </summary>
        /// <param name="id">Id record to update</param>
        /// <param name="item">Instance of patched object</param>
        /// <returns></returns>
        public async Task UpdateItemAsync(string id, ProcessingTask item)
        {
            await _container.UpsertItemAsync<ProcessingTask>(item, new PartitionKey(id));
        }
    }
}

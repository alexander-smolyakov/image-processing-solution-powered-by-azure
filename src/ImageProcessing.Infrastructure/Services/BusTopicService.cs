// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Messaging.ServiceBus;
using ImageProcessing.Core.Entities;

namespace ImageProcessing.Infrastructure.Services
{
    public class BusTopicService : IBusTopicService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public BusTopicService(string connectionString, string topicName)
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(topicName);
        }

        /// <summary>
        /// Send topic to service bus
        /// </summary>
        /// <param name="task">Object payload</param>
        /// <returns></returns>
        public async Task SendMessagesAsync(ProcessingTask task)
        {
            var message = new ServiceBusMessage(task.Id.ToString());
            await _sender.SendMessageAsync(message);
        }
    }
}

// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Azure.Messaging.ServiceBus;
using ImageProcessing.Core.Entities;

namespace ImageProcessing.Infrastructure.Services
{
    public class BusTopicService : IBusTopicService
    {
        private readonly ServiceBusClient _busClient;
        private readonly ServiceBusSender _busSender;

        public BusTopicService(string connectionString, string topicName)
        {
            _busClient = new ServiceBusClient(connectionString);
            _busSender = _busClient.CreateSender(topicName);
        }

        public async Task SendMessagesAsync(ProcessingTask task)
        {
            var message = new ServiceBusMessage(task.Id.ToString());
            await _busSender.SendMessageAsync(message);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubsPublish
{
    class Program
    {  
        static Random rand = new Random();

        private const string connectionString = "";
            
        private const string eventHubName = "";
             
        static async Task Main(string[] args)
        {  
            await SendToSamePartition();                   
        }

        static async Task SendToRandomPartition()
        {            
            await using var producerClient = 
                new EventHubProducerClient(connectionString, eventHubName);

            using EventDataBatch eventBatch = 
                await producerClient.CreateBatchAsync();
                            
            for (int i = 0; i < 100; i++)
            {
                double waterTemp = (rand.NextDouble()) * 100;
                int coffeeTypeIndex = rand.Next(2);

                var coffeeMachineData = new CoffeeData
                { 
                    WaterTemperature = waterTemp, 
                    ReadingTime = DateTime.Now, 
                    CoffeeType = CoffeeData.AllCoffeeTypes[coffeeTypeIndex]
                };

                var coffeeMachineDataBytes = 
                    JsonSerializer.SerializeToUtf8Bytes(coffeeMachineData);

                var eventData = new EventData(coffeeMachineDataBytes);

                if (!eventBatch.TryAdd(eventData))
                {
                    throw new Exception("Cannot add coffee machine data to random batch");
                }
            }

            await producerClient.SendAsync(eventBatch); 

            Console.WriteLine("Wrote events to random partitions");           
        }
        
        static async Task SendToSamePartition()
        {            
            await using var producerClient = 
                new EventHubProducerClient(connectionString, eventHubName);
            
            // can also do this with EventDataBatch - but showing another way
            
            List<EventData> eventBatch = new List<EventData>();
                
            for (int i = 0; i < 100; i++)
            {
                double waterTemp = (rand.NextDouble()) * 100;
                int coffeeTypeIndex = rand.Next(2);

                var coffeeMachineData = new CoffeeData
                { 
                    WaterTemperature = waterTemp, 
                    ReadingTime = DateTime.Now, 
                    CoffeeType = CoffeeData.AllCoffeeTypes[coffeeTypeIndex]
                };
            
                var coffeeMachineDataBytes = 
                    JsonSerializer.SerializeToUtf8Bytes(coffeeMachineData);

                var eventData = new EventData(coffeeMachineDataBytes);
                             
                eventBatch.Add(eventData);                
            }

            var options = new SendEventOptions();
            options.PartitionKey = "device1";
            
            await producerClient.SendAsync(eventBatch, options);

            Console.WriteLine("Wrote events to single partition");
        }

        
    }    
}

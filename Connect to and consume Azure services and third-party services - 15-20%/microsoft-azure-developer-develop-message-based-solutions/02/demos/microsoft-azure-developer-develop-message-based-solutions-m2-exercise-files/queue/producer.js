const { ServiceBusClient } = require("@azure/service-bus");

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_SERVICE_BUS_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_SERVICE_BUS_CONNECTION_STRING must be set.');
}

// Create a sender for the queue
const serviceBusClient = ServiceBusClient.createFromConnectionString(connectionString);
const queueClient = serviceBusClient.createQueueClient('testqueue');
const queueSender = queueClient.createSender();

// This function inserts a message (based on the message argument) into the queue
const insertMessageBatchIntoQueue = async (messages) => {
  const messagesToSend = messages.map(m => ({ body: m, contentType: 'text/plain' }));
  try {
    await queueSender.sendBatch(messagesToSend);
    console.log(`Inserted ${messages.length} messages...`);
  } catch(err) {
    console.error(err);
  }
}

// This is our main function for this program
const BATCH_SIZE = 20;
const main = async () => {
  const messages = [];
  for(let i = 0; i < BATCH_SIZE; i++) {
    messages.push(`Message ${i + 1}`);
  }
  await insertMessageBatchIntoQueue(messages);
  await queueClient.close();
  await serviceBusClient.close();
}

main();

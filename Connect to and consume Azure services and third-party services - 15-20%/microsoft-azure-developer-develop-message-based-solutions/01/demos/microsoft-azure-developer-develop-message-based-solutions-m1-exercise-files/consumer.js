const { QueueServiceClient } = require("@azure/storage-queue");

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_STORAGE_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_STORAGE_CONNECTION_STRING must be set.');
}

// Get the queueClient for the queue
const queueServiceClient = new QueueServiceClient.fromConnectionString(connectionString);
const queueClient = queueServiceClient.getQueueClient('testqueue');

// This is a utility function that just allows us to insert a delay (using
// promises) before we check for messages again
const delay = async (ms) => {
  return new Promise(res => setTimeout(res, ms));
}

// Each message received gets passed into this function which logs the message
// and deletes it
const processMessage = async (message) => {
  console.dir(message);
  queueClient.deleteMessage(message.messageId, message.popReceipt);
}

// This is our main function for this program
const main = async () => {
  const receivedMessages = await queueClient.receiveMessages();
  receivedMessages.receivedMessageItems.forEach(processMessage);
  await delay(1000);
  await main();
}

main();

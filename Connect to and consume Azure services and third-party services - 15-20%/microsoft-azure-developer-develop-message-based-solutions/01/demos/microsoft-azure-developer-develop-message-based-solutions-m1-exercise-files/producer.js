const { QueueServiceClient } = require("@azure/storage-queue");

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_STORAGE_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_STORAGE_CONNECTION_STRING must be set.');
}

// Get the queueClient for the queue
const queueServiceClient = new QueueServiceClient.fromConnectionString(connectionString);
const queueClient = queueServiceClient.getQueueClient('testqueue');

// This function inserts a message (based on the message argument) into the queue
const insertMessageIntoQueue = async (message) => {
  try {
    await queueClient.sendMessage(message);
    console.log(`${message} inserted...`);
  } catch(err) {
    console.error(err);
  }
}

// This is a utility function that just allows us to insert a delay (using
// promises) before we check for messages again
const delay = async (ms) => {
  return new Promise(res => setTimeout(res, ms));
}

// This is our main function for this program
let messageNumber = 1;
const main = async () => {
  await insertMessageIntoQueue(`Message ${messageNumber}`);
  messageNumber++;
  await delay(2000);
  await main();
}

main();

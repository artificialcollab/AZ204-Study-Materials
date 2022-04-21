const { ServiceBusClient, ReceiveMode } = require("@azure/service-bus");

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_SERVICE_BUS_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_SERVICE_BUS_CONNECTION_STRING must be set.');
}

// Create a sender for the queue
const serviceBusClient = ServiceBusClient.createFromConnectionString(connectionString);
const queueClient = serviceBusClient.createQueueClient('testqueue');
const queueReceiver = queueClient.createReceiver(ReceiveMode.peekLock);

// This is a utility function that just allows us to insert a delay (using
// promises) before we check for messages again
const delay = async (ms) => {
  return new Promise(res => setTimeout(res, ms));
}

// This is our main function for this program
let currentFetch = 1;
const timesToFetch = 4;
const main = async () => {
  // We'll fetch a batch of messages and then process them each
  while (currentFetch <= timesToFetch) {
    // We are going to get 5 messages and wait up to 5 seconds for them
    const receivedMessages = await queueReceiver.receiveMessages(5, 5);
    if(receivedMessages.length < 1) {
      console.log(`No messages to retrieve.`);
    } else {
      receivedMessages.forEach(m => console.log(m.body));
      for(const m of receivedMessages) {
        console.log(`Completing message: ${m.body}`);
        // This deletes the message from the queue
        await m.complete();
      }
    }
    currentFetch++;
    await delay(2000);
  }
  await queueClient.close();
  await serviceBusClient.close();
}

main();

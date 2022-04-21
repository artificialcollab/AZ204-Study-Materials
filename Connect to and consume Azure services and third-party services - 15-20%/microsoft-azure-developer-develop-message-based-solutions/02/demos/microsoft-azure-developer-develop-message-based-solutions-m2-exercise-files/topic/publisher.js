const { ServiceBusClient } = require("@azure/service-bus");

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_SERVICE_BUS_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_SERVICE_BUS_CONNECTION_STRING must be set.');
}

// Create a sender for the queue
const serviceBusClient = ServiceBusClient.createFromConnectionString(connectionString);
const topicClient = serviceBusClient.createTopicClient('testtopic');
const topicSender = topicClient.createSender();

// This is a utility function that just allows us to insert a delay (using
// promises) before we check for messages again
const delay = async (ms) => {
  return new Promise(res => setTimeout(res, ms));
}

// This is our main function for this program
let messageNumber = 1;
const NUMBER_MESSAGES = 50;
const main = async () => {
  while(messageNumber <= NUMBER_MESSAGES) {
    console.log(`Sending Message ${messageNumber}`);
    await topicSender.send({ body: `Message ${messageNumber}`, label: `${(messageNumber % 2) ? 'Odd' : 'Even'}` });
    messageNumber++;
    await delay(1500);
  }
  await topicClient.close();
  await serviceBusClient.close();
}

main();

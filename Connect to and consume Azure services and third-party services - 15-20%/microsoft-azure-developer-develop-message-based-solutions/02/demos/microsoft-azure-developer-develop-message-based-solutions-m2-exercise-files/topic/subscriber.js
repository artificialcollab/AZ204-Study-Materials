/*

INSTRUCTIONS

Before this sample will work, you will need to create two subscriptions in either the portal
or with the CLI.  One should be named 'odd' and one named 'even'. 

You can create this with the CLI by calling:

az servicebus topic subscription create --resource-group <myresourcegroup> --namespace-name <mynamespace> --topic-name testtopic --name Odd
az servicebus topic subscription create --resource-group <myresourcegroup> --namespace-name <mynamespace> --topic-name testtopic --name Even

*/
const { ServiceBusClient, ReceiveMode } = require("@azure/service-bus");

// Get the subscription name from the command line argument
const subValues = [ 'Odd', 'Even' ];
if(process.argv.length < 3 || !subValues.includes(process.argv[2])) {
  throw new Error('You must pass in either odd or even as an argument to this program');
}
const subscriptionName = process.argv[2];

// Get connection string (for the associated storage account)
const connectionString = process.env.AZURE_SERVICE_BUS_CONNECTION_STRING;
if(!connectionString) {
  throw new Error('Environment variable AZURE_SERVICE_BUS_CONNECTION_STRING must be set.');
}

// Removes all current rules, and adds a new rule that filters based on the label
const setupSubscription = async (client) => {
  const rules = await client.getRules();
  for (let i = 0; i < rules.length; i++) {
    await client.removeRule(rules[i].name);
  }
  await client.addRule('labelRule', { label: subscriptionName });
}

// Create a sender for the queue
const serviceBusClient = ServiceBusClient.createFromConnectionString(connectionString);
const subscriptionClient = serviceBusClient.createSubscriptionClient('testtopic', subscriptionName);

// This is our main function for this program
let shouldRecover = true;

const main = async () => {
  await setupSubscription(subscriptionClient);
  const subscriptionReceiver = subscriptionClient.createReceiver(ReceiveMode.peekLock);
  
  const receiverPromise = new Promise((_resolve, reject) => {
    // Handle a message
    const onMessage = async (message) => {
      console.log(`Received message: ${message.body}`);
      await message.complete();
    }

    // On an error
    const onError = err => {
      console.log(`Error: ${err}`);
      reject(err);
    }

    // Setup the handlers
    subscriptionReceiver.registerMessageHandler(onMessage, onError, { autoComplete: false });
  })

  await receiverPromise;
  console.log('Ending subscription. Closing connections...');
  await queueClient.close();
  await serviceBusClient.close();
}

main();

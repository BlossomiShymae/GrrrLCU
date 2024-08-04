using BlossomiShymae.GrrrLCU;

var client = Connector.CreateLcuWebsocketClient();

// Subscribe to any events.
client.EventReceived.Subscribe(msg =>
{
    Console.WriteLine(msg?.Data?.Uri);
});
client.DisconnectionHappened.Subscribe(msg => 
{
    if (msg.Exception != null) throw msg.Exception;
});
client.ReconnectionHappened.Subscribe(msg =>
{
    Console.WriteLine(msg.Type);
});

// This starts the client in a background thread. You will need an event loop
// to listen to messages.
await client.Start();

// Subscribe to every event that the League Client sends.
var message = new EventMessage(RequestType.Subscribe, EventMessage.Kinds.OnJsonApiEvent);
client.Send(message);

// We will need an event loop for the background thread to process.
// You may close at any time with Ctrl+C or similar chord.
while(true) await Task.Delay(TimeSpan.FromSeconds(1));

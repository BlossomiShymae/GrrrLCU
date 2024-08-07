# GrrrLCU

[![NuGet Stable](https://img.shields.io/nuget/v/BlossomiShymae.GrrrLCU.svg?style=flat-square&logo=nuget&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/) [![NuGet Downloads](https://img.shields.io/nuget/dt/BlossomiShymae.GrrrLCU?style=flat-square&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/)

GrrrLCU is a wrapper for the LCU API which is unofficially provided by Riot Games.

This library is currently compatible with .NET 8 and higher for Windows.

## Contributors

<a href="https://github.com/BlossomiShymae/GrrrLCU/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=BlossomiShymae/GrrrLCU" />
</a>

## Requesting the LCU

Requesting with GrrrLCU is simple:

```csharp
var response = await Connector.SendAsync(HttpMethod.Get, "/lol-summoner/v1/current-summoner");

var me = await response.Content.ReadFromJsonAsync<Summoner>();
```

Simpler for GET requests:

```csharp
var response = await Connector.GetAsync("/lol-summoner/v1/current-summoner");

var me = await response.Content.ReadFromJsonAsync<Summoner>();
```

Simplest for GET requests:

```csharp
var me = await Connector.GetFromJsonAsync<Summoner>("/lol-summoner/v1/current-summoner");
```

Utilities:

```csharp
var processInfo = Connector.GetProcessInfo();
var riotAuthentication = new RiotAuthentication(processInfo.RemotingAuthToken);
```

## WebSockets

This library uses the `Websocket.Client` wrapper, which comes with built-in reconnection and error handling.

Create a client:

```csharp
var client = Connector.CreateLcuWebsocketClient();
```

Listen to events, disconnections, or reconnection messages:

```csharp
using System; // Include to avoid compiler errors CS1503, CS1660
              // You may or may not need this

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
```

Use it:

```csharp
// This starts the client in a background thread. You will need an event loop
// to listen to messages.
await client.Start();

// Subscribe to every event that the League Client sends.
var message = new EventMessage(RequestType.Subscribe, EventMessage.Kinds.OnJsonApiEvent);
client.Send(message);

// We will need an event loop for the background thread to process.
// You may close at any time with Ctrl+C or similar chord.
while(true) await Task.Delay(TimeSpan.FromSeconds(1));
```
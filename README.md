# GrrrLCU

[![NuGet Stable](https://img.shields.io/nuget/v/BlossomiShymae.GrrrLCU.svg?style=flat-square&logo=nuget&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/) [![NuGet Downloads](https://img.shields.io/nuget/dt/BlossomiShymae.GrrrLCU?style=flat-square&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/)

GrrrLCU is a REST library for the LCU API which is unofficially provided by Riot Games.

This library is currently compatible with .NET 8 and higher.

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
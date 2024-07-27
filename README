# GrrrLCU

[![NuGet Stable](https://img.shields.io/nuget/v/BlossomiShymae.GrrrLCU.svg?style=flat-square&logo=nuget&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/) [![NuGet Downloads](https://img.shields.io/nuget/dt/BlossomiShymae.GrrrLCU?style=flat-square&logoColor=black&labelColor=69ffbe&color=77077a)](https://www.nuget.org/packages/BlossomiShymae.GrrrLCU/)

GrrrLCU is a REST library for the LCU API which is unofficially provided by Riot Games.

This library is currently compatible with .NET 8 and higher.

## Contributors

<a href="https://github.com/BlossomiShymae/GrrrLCU/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=BlossomiShymae/GrrrLCU" />
</a>

## Making a request to the LCU

Making a request with GrrrLCU is simple:
```csharp
// Create your request message. :3
var request = new HttpRequestMessage(HttpMethod.Get, "/lol-summoner/v1/current-summoner");

// Send a request! :o
var response = await Connector.SendAsync(request);

// Get your data. Provide your types. :>
var me = await response.Content.ReadFromJsonAsync<Summoner>();
```
As always, don't forget to use exception handling!
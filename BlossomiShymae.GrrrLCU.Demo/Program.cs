using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BlossomiShymae.GrrrLCU;
using Spectre.Console;
using Spectre.Console.Json;
using Websocket.Client;

try
{
    var httpClient = new HttpClient();
    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true};

    var controller = new Controller(httpClient, jsonSerializerOptions);
    await controller.StartAsync();
} 
catch (InvalidOperationException ex)
{
    if (ex.Message.Contains("LCUx"))
    {
        Console.WriteLine("This demo must have the LCU running.");
    }
    else
    {
        throw;
    }
}

public class Controller
{
    public HttpClient HttpClient { get; }
    public JsonSerializerOptions JsonSerializerOptions { get; }

    public Controller(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
    {
        HttpClient = httpClient;
        JsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task StartAsync()
    {
        var summoner = await GetSummonerAsync();
        var userResource = await GetUserResourceAsync();
        var championMasteries = (await GetChampionMasteriesAsync()).GetRange(0, 3);
        var championSummaries = await GetChampionSummariesAsync();

        var summonerPanel = new Panel(summoner.ToString())
        {
            Header = new PanelHeader("Summoner")
        };

        var userResourcePanel = new Panel(userResource.ToString())
        {
            Header = new PanelHeader("User Resource")
        };

        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Level");
        table.AddColumn("Points");
        foreach (var mastery in championMasteries) 
            table.AddRow(championSummaries.Find(s => s.Id == mastery.ChampionId)!.Name, $"{mastery.ChampionLevel}", $"{mastery.ChampionPoints}");

        AnsiConsole.Write(summonerPanel);
        AnsiConsole.Write(userResourcePanel);
        AnsiConsole.Write(table);

        var playerNotificationResource = new PlayerNotificationResource()
        {
            TitleKey = "pre_translated_title",
            DetailKey = "pre_translated_details",
            Data = new
            {
                Title = "GrrrLCU",
                Details = "This is a test notification from GrrrLCU."
            }
        };
        await Connector.SendAsync(HttpMethod.Post, "/player-notifications/v1/notifications", JsonContent.Create(playerNotificationResource));

        AnsiConsole.Markup("[yellow]A test notification was sent to the LCU client. Check it out.[/]\n");

        var client = Connector.CreateLcuWebsocketClient();

        // Subscribe to any events.
        client.EventReceived.Subscribe(Client_EventReceived);
        client.DisconnectionHappened.Subscribe(Client_DisconnectionHappened);
        client.ReconnectionHappened.Subscribe(Client_ReconnectionHappened);

        // This starts the client in a background thread. You will need an event loop
        // to listen to messages.
        await client.Start();

        // Subscribe to every event that the League Client sends.
        var message = new EventMessage(RequestType.Subscribe, EventMessage.Kinds.OnJsonApiEvent);
        client.Send(message);

        // We will need an event loop for the background thread to process.
        // You may close at any time with Ctrl+C or similar chord.
        while(true) await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private void Client_ReconnectionHappened(ReconnectionInfo info)
    {
        // Do nothing
    }

    private void Client_DisconnectionHappened(DisconnectionInfo info)
    {
        if (info.Exception != null) throw info.Exception;
    }

    private void Client_EventReceived(EventMessage message)
    {
        var jsonPanel = new Panel(new JsonText(JsonSerializer.Serialize(message, JsonSerializerOptions)))
        {
            Header = new PanelHeader("EventMessage"),
        };
        AnsiConsole.Write(jsonPanel);
    }

    private async Task<Summoner> GetSummonerAsync()
    {
        var summoner = await Connector.GetFromJsonAsync<Summoner>("/lol-summoner/v1/current-summoner");
        return summoner ?? throw new NullReferenceException("Summoner is null");
    }

    private async Task<UserResource> GetUserResourceAsync()
    {
        var userResource = await Connector.GetFromJsonAsync<UserResource>("/lol-chat/v1/me");
        return userResource ?? throw new NullReferenceException("User resource is null");
    }

    private async Task<List<ChampionMastery>> GetChampionMasteriesAsync()
    {
        var championMasteries = await Connector.GetFromJsonAsync<List<ChampionMastery>>("/lol-champion-mastery/v1/local-player/champion-mastery");
        return championMasteries ?? throw new NullReferenceException("Champion masteries is null");
    }

    private async Task<List<ChampionSummary>> GetChampionSummariesAsync()
    {
        var championSummaries = await HttpClient.GetFromJsonAsync<List<ChampionSummary>>("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json", JsonSerializerOptions);
        return championSummaries ?? throw new NullReferenceException("Champion summaries is null");
    }
}

class Summoner {
    public ulong SummonerId { get; set; }
    public ulong AccountId { get; set; }
    public string Puuid { get; set; } = string.Empty;
    public string InternalName { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public string TagLine { get; set; } = string.Empty;
    public uint SummonerLevel { get; set; }

    public override string ToString()
    {
        var text = $"Id: {SummonerId}";
        text += $"\nAccount Id: {AccountId}";
        text += $"\nPuuid: {Puuid}";
        text += $"\nInternal Name: {InternalName}";
        text += $"\nRiot Id: {GameName}#{TagLine}";
        text += $"\nLevel: {SummonerLevel}";
        return text;
    }
}

class UserResource
{
    public string Availability { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string PlatformId { get; set; } = string.Empty;
    public string StatusMessage { get; set; } = string.Empty;

    public override string ToString()
    {
        var text = $"Id: {Id}";
        text += $"\nPlatform Id: {PlatformId}";
        text += $"\nAvailability: {Availability}";
        text += $"\nStatus: {StatusMessage}";
        return text;
    }
}

class ChampionMastery
{
    public int ChampionId { get; set;}
    public int ChampionLevel { get; set; }
    public int ChampionPoints { get; set; }
}

class PlayerNotificationResource
{
    public string BackgroundUrl { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public bool Critical { get; set; }
    public object Data { get; set; } = new { };
    public string DetailKey { get; set; } = string.Empty;
    public string Expires { get; set; } = string.Empty;
    public string IconUrl { get; set; } =  string.Empty;
    public ulong Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string TitleKey { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Dismissable { get; set; } 
}

class ChampionSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
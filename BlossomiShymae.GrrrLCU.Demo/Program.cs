using System.Net.Http.Json;
using System.Text.Json;
using System.Timers;
using BlossomiShymae.GrrrLCU;
using Spectre.Console;
using Spectre.Console.Json;
using Websocket.Client;
using Timer = System.Timers.Timer;

var lcuHttpClient = Connector.GetLcuHttpClientInstance();
var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true};

var controller = new Controller(lcuHttpClient, jsonSerializerOptions);

var connectionTimer = new Timer(TimeSpan.FromSeconds(5));

connectionTimer.Elapsed += OnElapsed;
connectionTimer.Enabled = true;
connectionTimer.Start();

while (controller.IsRunning) await Task.Delay(TimeSpan.FromSeconds(1));


async void OnElapsed(object? sender, ElapsedEventArgs elapsedEventArgs)
{
    Console.WriteLine("Finding LCUx process...");
    if (ProcessFinder.IsPortOpen())
    {
        Console.WriteLine("Connecting to LCUx process...");
        connectionTimer.Elapsed -= OnElapsed;
        connectionTimer.Stop();
        connectionTimer.Dispose();

        await controller.StartAsync();   
    }
}

public class Controller(LcuHttpClient lcuHttpClient, JsonSerializerOptions jsonSerializerOptions)
{
    public LcuHttpClient LcuHttpClient { get; } = lcuHttpClient;
    public JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptions;
    public LcuWebsocketClient? LcuWebsocketClient { get; set; }
    public bool IsRunning { get; set; } = true;

    public async Task StartAsync()
    {
        var summoner = new Summoner();
        int retries = 3;
        // We must have retry logic here for GET /lol-summoner/v1/current-summoner as
        // it will return a 404 when connected too early...
        for (int i = 0; i < retries; i++)
        {
            var res = await LcuHttpClient.GetAsync("/lol-summoner/v1/current-summoner")
                ?? throw new Exception("Failed to get summoner");

            if (res.IsSuccessStatusCode)
            {
                summoner = await res.Content.ReadFromJsonAsync<Summoner>()
                    ?? throw new Exception("Failed to get summoner");
                break;
            }
            
            if (i == retries - 1) throw new Exception("Failed to connect to LCUx process...");

            var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
            Console.WriteLine($"Backing off for {delay.TotalSeconds} second(s)...");
            await Task.Delay(delay);
        }

        var userResource = await LcuHttpClient.GetFromJsonAsync<UserResource>("/lol-chat/v1/me")
            ?? throw new Exception("Failed to get user resource");
        var championMasteries = (await LcuHttpClient.GetFromJsonAsync<List<ChampionMastery>>("/lol-champion-mastery/v1/local-player/champion-mastery")
            ?? throw new Exception("Failed to get champion masteries")).GetRange(0, 3);
        var championSummaries = await LcuHttpClient.GetFromJsonAsync<List<ChampionSummary>>("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json", JsonSerializerOptions)
            ?? throw new Exception("Failed to get champion summaries");

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
        await LcuHttpClient.PostAsJsonAsync("/player-notifications/v1/notifications", playerNotificationResource);

        AnsiConsole.Markup("[yellow]A test notification was sent to the LCU client. Check it out.[/]\n");

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

        await InitializeWebsocketAsync();
    }

    private async Task InitializeWebsocketAsync()
    {
        var client = LcuWebsocketClient = Connector.CreateLcuWebsocketClient();

        client.EventReceived.Subscribe(Client_EventReceived);
        client.DisconnectionHappened.Subscribe(Client_DisconnectionHappened);
        client.ReconnectionHappened.Subscribe(Client_ReconnectionHappened);

        await client.Start();

        var message = new EventMessage(RequestType.Subscribe, EventMessage.Kinds.OnJsonApiEvent);
        client.Send(message);
    }

    private void Client_ReconnectionHappened(ReconnectionInfo info)
    {
        // Do nothing
    }

    private void Client_DisconnectionHappened(DisconnectionInfo info)
    {
       LcuWebsocketClient?.Dispose();
       Console.WriteLine("Disconecting from LCUx process...");
       IsRunning = false;
    }

    private void Client_EventReceived(EventMessage message)
    {
        var jsonPanel = new Panel(new JsonText(JsonSerializer.Serialize(message, JsonSerializerOptions)))
        {
            Header = new PanelHeader("EventMessage"),
        };
        AnsiConsole.Write(jsonPanel);
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

class Status
{
    public bool Ready { get; set; }
}
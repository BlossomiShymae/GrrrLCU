using System.Diagnostics;
using System.Net.Http.Json;

using BlossomiShymae.Briar.GameClient;
using BlossomiShymae.Briar.Rest;
using BlossomiShymae.Briar.Utils.Behaviors;

using Xunit.Abstractions;

namespace BlossomiShymae.Briar.Tests
{
    public class ConnectorTest
    {
        private readonly ITestOutputHelper _output;
        private readonly LcuHttpClient _lcuHttpClient;
        private readonly GameHttpClient _gameHttpClient;

        public ConnectorTest(ITestOutputHelper output)
        {
            _output = output;
            _lcuHttpClient = Connector.GetLcuHttpClientInstance();
            _gameHttpClient = Connector.GetGameHttpClientInstance();
        }

        [Fact]
        [Trait("Method", "Lockfile")]
        public void LockfileTest()
        {
            var process = Process.GetProcessesByName("LeagueClientUx").First();

            new PortTokenWithLockfile().TryGet(process, out var token, out var port, out var ex);
            if (ex is Exception e)
            {
                throw e;
            }

            Assert.True(token != null);
            Assert.True(port > 0);
        }

        [Fact]
        [Trait("Method", "ProcessList")]
        public void ProcessListTest()
        {
            var process = Process.GetProcessesByName("LeagueClientUx").First();

            new PortTokenWithProcessList().TryGet(process, out var token, out var port, out var ex);
            if (ex is Exception e)
            {
                throw e;
            }

            Assert.True(token != null);
            Assert.True(port > 0);
        }

        [Fact]
        [Trait("API", "LCU")]
        public async Task LcuSendAsyncTest()
        {
            var response = await _lcuHttpClient.SendAsync(new(HttpMethod.Get, "/lol-summoner/v1/current-summoner"));

            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        [Trait("API", "LCU")]
        public async Task LcuGetAsyncTest()
        {
            var response = await _lcuHttpClient.GetAsync("/lol-summoner/v1/current-summoner");

            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        [Trait("API", "LCU")]
        public async Task LcuGetFromJsonAsyncTest()
        {
            var data = await _lcuHttpClient.GetFromJsonAsync<Summoner>("/lol-summoner/v1/current-summoner");
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        [Trait("API", "LCU")]
        public async Task LcuGetWithParamsTest()
        {
            var indexBegin = 10;
            var indexEnd = 11;

            var data = await _lcuHttpClient.GetFromJsonAsync<MatchHistoryList>($"/lol-match-history/v1/products/lol/current-summoner/matches?begIndex={indexBegin}&endIndex={indexEnd}");

            Assert.True(data != null);
            Assert.Equal(indexBegin, data.Games.GameIndexBegin);
            Assert.Equal(indexEnd, data.Games.GameIndexEnd);
        }

        [Fact]
        [Trait("API", "Game")]
        public async Task GameGetAsyncTest()
        {
            var response = await _gameHttpClient.GetAsync("/liveclientdata/activeplayername");

            var data = await response.Content.ReadFromJsonAsync<string>();
            _output.WriteLine($"{data}");

            Assert.True(data != null);
        }

        [Fact]
        [Trait("API", "Game")]
        public async Task GameGetFromJsonAsyncTest()
        {
            var data = await _gameHttpClient.GetFromJsonAsync<string>("/liveclientdata/activeplayername");
            _output.WriteLine($"{data}");

            Assert.True(data != null);
        }
    }

    public class Summoner
    {
        public required string GameName { get; set; }
    }

    public class MatchHistoryList
    {
        public required MatchHistoryGameList Games { get; set; }
    }

    public class MatchHistoryGameList
    {
        public required long GameIndexBegin { get; set; }
        
        public required long GameIndexEnd { get; set; }
    }
}
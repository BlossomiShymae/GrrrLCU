using System.Net.Http.Json;
using BlossomiShymae.Briar.Rest;
using Xunit.Abstractions;

namespace BlossomiShymae.Briar.Tests
{
    public class ConnectorTest
    {
        private readonly ITestOutputHelper _output;
        private readonly LcuHttpClient _lcuHttpClient;

        public ConnectorTest(ITestOutputHelper output)
        {
            _output = output;
            _lcuHttpClient = Connector.GetLcuHttpClientInstance();
        }

        [Fact]
        public async Task SendAsyncTest()
        {
            var response = await _lcuHttpClient.SendAsync(new(HttpMethod.Get, "/lol-summoner/v1/current-summoner"));
            
            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        public async Task GetAsyncTest()
        {
            var response = await _lcuHttpClient.GetAsync("/lol-summoner/v1/current-summoner");

            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        public async Task GetFromJsonAsyncTest()
        {
            var data = await _lcuHttpClient.GetFromJsonAsync<Summoner>("/lol-summoner/v1/current-summoner");
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }
    }

    public class Summoner
    {
        public required string GameName { get; set; }
    }
}
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace BlossomiShymae.GrrrLCU.Tests
{
    public class ConnectorTest
    {
        private readonly ITestOutputHelper _output;

        public ConnectorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SendAsyncTest()
        {
            var response = await Connector.SendAsync(HttpMethod.Get, new Uri("/lol-summoner/v1/current-summoner"));
            
            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{nameof(Connector.SendAsync)}: {data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        public async Task GetAsyncTest()
        {
            var response = await Connector.GetAsync(new Uri("/lol-summoner/v1/current-summoner"));

            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{nameof(Connector.GetAsync)}: {data?.GameName}");

            Assert.True(data != null);
        }

        [Fact]
        public async Task GetFromJsonAsyncTest()
        {
            var data = await Connector.GetFromJsonAsync<Summoner>(new Uri("/lol-summoner/v1/current-summoner"));
            _output.WriteLine($"{nameof(Connector.GetFromJsonAsync)}: {data?.GameName}");

            Assert.True(data != null);
        }
    }

    public class Summoner
    {
        public required string GameName { get; set; }
    }
}
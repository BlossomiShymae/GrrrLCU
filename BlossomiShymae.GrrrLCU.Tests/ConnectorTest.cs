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
        public async Task RequestTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/lol-summoner/v1/current-summoner");

            var response = await Connector.SendAsync(request);
            _output.WriteLine($"{response}");
            
            var data = await response.Content.ReadFromJsonAsync<Summoner>();
            _output.WriteLine($"{data?.GameName}");

            Assert.True(data != null);
        }
    }

    public class Summoner
    {
        public required string GameName { get; set; }
    }
}
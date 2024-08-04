using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Connector to exchange requests with the League Client.
    /// </summary>
    public static class Connector
    {
        internal static HttpClient HttpClient { get; } = new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        internal static JsonSerializerOptions JsonSerializerOptions { get; } = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Get information of the League Client process.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ProcessInfo GetProcessInfo()
        {
            ProcessInfo? processInfo = null;

            foreach (var process in Process.GetProcesses())
            {
                switch(process.ProcessName)
                {
                    case "LeagueClientUx":
                        processInfo = new ProcessInfo(process);
                        break;
                    default:
                        break;
                }

                if (processInfo != null) break;
            }

            return processInfo ?? throw new InvalidOperationException("Failed to find LCUx process.");
        }

        /// <summary>
        /// Get an Uri of the running League Client API.
        /// </summary>
        /// <param name="appPort"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Uri GetLeagueClientUri(int appPort, string path)
        {
            return new Uri($"https://127.0.0.1:{appPort}{path}");
        }

        /// <summary>
        /// Set the timeout for the internal HttpClient.
        /// </summary>
        /// <param name="timeSpan"></param>
        public static void SetTimeout(TimeSpan timeSpan)
        {
            HttpClient.Timeout = timeSpan;
        }

        /// <summary>
        /// Send a request to the League Client.
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string path, CancellationToken cancellationToken = default)
        {
            var processInfo = GetProcessInfo();
            var riotAuthentication = new RiotAuthentication(processInfo.RemotingAuthToken);

            var request = new HttpRequestMessage(httpMethod, GetLeagueClientUri(processInfo.AppPort, path));
            request.Headers.Authorization = riotAuthentication.ToAuthenticationHeaderValue();
                        
            var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Send a GET request to the League Client.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var response = await SendAsync(HttpMethod.Get, path, cancellationToken).ConfigureAwait(false);
            
            return response;
        }

        /// <summary>
        /// Send a GET request to the League Client for deserialized JSON data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> GetFromJsonAsync<T>(string path, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(path, cancellationToken).ConfigureAwait(false);
            
            var data = await response.Content.ReadFromJsonAsync<T>(options ?? JsonSerializerOptions, cancellationToken).ConfigureAwait(false);

            return data;
        }

        /// <summary>
        /// Create a Websocket client with logger for the League Client.
        /// </summary>
        /// <returns></returns>
        public static LcuWebsocketClient CreateLcuWebsocketClient(ILogger<WebsocketClient>? logger)
        {
            var processInfo = GetProcessInfo();
            var riotAuthentication = new RiotAuthentication(processInfo.RemotingAuthToken);
            var uri = new Uri($"wss://127.0.0.1:{processInfo.AppPort}/");
            ClientWebSocket factory() => new()
            {
                Options =
                {
                    Credentials = riotAuthentication.ToNetworkCredential(),
                    RemoteCertificateValidationCallback = (a, b, c, d) => true,
                },
            };
            
            var client = new LcuWebsocketClient(uri, logger, factory);

            return client;
        }

        /// <summary>
        /// Create a Websocket client for the League client.
        /// </summary>
        /// <returns></returns>
        public static LcuWebsocketClient CreateLcuWebsocketClient() =>
            CreateLcuWebsocketClient(null);
    }
}
using System.Diagnostics;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Connector to exchange requests with the League Client.
    /// </summary>
    public static class Connector
    {
        internal static HttpClient HttpClient { get; set; } = new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        internal static ProcessInfo GetProcessInfo()
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
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, Uri requestUri, CancellationToken cancellationToken = default)
        {
            var processInfo = GetProcessInfo();
            var riotAuthentication = new RiotAuthentication(processInfo.RemotingAuthToken);

            var request = new HttpRequestMessage(httpMethod, new Uri($"https://127.0.0.1:{processInfo.AppPort}{requestUri}"));
            request.Headers.Authorization = riotAuthentication.ToAuthenticationHeaderValue();
                        
            var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
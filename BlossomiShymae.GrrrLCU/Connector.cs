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

        internal static SemaphoreSlim Lock { get; } = new(1,1);

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
        /// <param name="request"></param>
        /// <param name="processInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var processInfo = GetProcessInfo();
            var riotAuthentication = new RiotAuthentication(processInfo.RemotingAuthToken);

            // Semaphore is used to avoid an unhandled exception from modifying the HttpClient between requests.
            await Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                HttpClient.BaseAddress = new Uri($"https://127.0.0.1:{processInfo.AppPort}/");
                HttpClient.DefaultRequestHeaders.Authorization = riotAuthentication.ToAuthenticationHeaderValue();
                

                var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                return response;
            }
            finally
            {
                Lock.Release();
            }
           
        }
    }
}
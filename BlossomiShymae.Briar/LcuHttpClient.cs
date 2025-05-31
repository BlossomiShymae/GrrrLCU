using System.Text.Json;

namespace BlossomiShymae.Briar
{
    /// <summary>
    /// A simple HTTP client for the League Client.
    /// </summary>
    public class LcuHttpClient : HttpClient
    {
        private static readonly Lazy<LcuHttpClient> _instance = new(() => new LcuHttpClient(new()));
        internal static LcuHttpClient Instance => _instance.Value;

        private LcuHttpClientHandler _handler { get; }

        /// <summary>
        /// The information of the most recent League Client process.
        /// </summary>
        public ProcessInfo? ProcessInfo 
        {
            get => _handler.ProcessInfo;
            internal set 
            {
                _handler.ProcessInfo = value;
            }
        }

        /// <summary>
        /// The authentication of the most recent League Client process.
        /// </summary>
        public RiotAuthentication? RiotAuthentication => _handler.RiotAuthentication;

        internal LcuHttpClient(LcuHttpClientHandler lcuHttpClientHandler) : base(lcuHttpClientHandler) 
        { 
            _handler = lcuHttpClientHandler;
            BaseAddress = new("https://127.0.0.1"); // This is only done to make PrepareRequestMessage not throw.
        }   
    }
}
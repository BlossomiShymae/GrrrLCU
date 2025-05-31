namespace BlossomiShymae.Briar.GameClient
{
    /// <summary>
    /// A simple HTTP client for the Game Client.
    /// </summary>
    public class GameHttpClient : HttpClient
    {
        private static readonly Lazy<GameHttpClient> _instance = new(() => new GameHttpClient(new()));
        internal static GameHttpClient Instance => _instance.Value;

        internal GameHttpClient(GameHttpClientHandler gameHttpClientHandler) : base(gameHttpClientHandler)
        {
            BaseAddress = new("https://127.0.0.1"); // This is only done to make PrepareRequestMessage not throw.
        }
    }
}
namespace BlossomiShymae.Briar.WebSocket.Events
{
    /// <summary>
    /// Documented kinds for event messages.
    /// </summary>
    public sealed class EventKinds
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly EventKind OnJsonApiEvent = new() { Prefix = "OnJsonApiEvent" };
        public static readonly EventKind OnLcdsEvent = new() { Prefix = "OnLcdsEvent" };
        public static readonly EventKind OnLog = new() { Prefix = "OnLog" };
        public static readonly EventKind OnRegionLocaleChanged = new() { Prefix = "OnRegionLocaleChanged" };
        public static readonly EventKind OnServiceProxyAsyncEvent = new() { Prefix = "OnServiceProxyAsyncEvent" };
        public static readonly EventKind OnServiceProxyMethodEvent = new() { Prefix = "OnServiceProxyMethodEvent" };
        public static readonly EventKind OnServiceProxyUuidEvent = new() { Prefix = "OnServiceProxyUuidEvent" };
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
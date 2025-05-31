namespace BlossomiShymae.Briar
{
    /// <summary>
    /// The websocket operations available in a event message.
    /// </summary>
    public enum EventRequestType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Welcome = 0,
        Prefix = 1,
        Call = 2,
        CallResult = 3,
        CallError = 4,
        Subscribe = 5,
        Unsubscribe = 6,
        Publish = 7,
        Event = 8
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
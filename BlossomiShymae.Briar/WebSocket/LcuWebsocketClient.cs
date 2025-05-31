using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using BlossomiShymae.Briar.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace BlossomiShymae.Briar.WebSocket
{
    /// <summary>
    /// A simple websocket client for the League Client.
    /// </summary>
    public class LcuWebsocketClient : WebsocketClient
    {
        private readonly Subject<EventMessage> _eventReceivedSubject = new();

        /// <summary>
        /// Stream with received event message.
        /// </summary>
        public IObservable<EventMessage> EventReceived => _eventReceivedSubject.AsObservable();

        internal LcuWebsocketClient(Uri uri, ILogger<WebsocketClient>? logger, Func<ClientWebSocket>? factory = null) : base(uri, logger, factory)
        {
            MessageReceived.Subscribe(msg => 
            {
                if (!string.IsNullOrEmpty(msg.Text)) _eventReceivedSubject.OnNext(JsonSerializer.Deserialize<EventMessage>(msg.Text)!);
            });
        }

        /// <summary>
        /// Send event message to the websocket channel. It inserts the message to the queue
        /// and actual sending is done on another thread.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Send(EventMessage message)
        {
            return Send(JsonSerializer.Serialize(message));
        }
    }
}
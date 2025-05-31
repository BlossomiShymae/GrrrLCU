using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlossomiShymae.Briar.GameClient
{
    internal class GameHttpClientHandler : HttpClientHandler
    {
        public string BaseAddress => "https://127.0.0.1:2999";

        internal GameHttpClientHandler() : base()
        {
            ServerCertificateCustomValidationCallback = DangerousAcceptAnyServerCertificateValidator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            PrepareRequestMessage(request);
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            PrepareRequestMessage(request);
            return base.Send(request, cancellationToken);
        }

        private void PrepareRequestMessage(HttpRequestMessage request)
        {
            request.RequestUri = new Uri($"{request.RequestUri?.ToString().Replace("https://127.0.0.1", BaseAddress)}");
        }
    }
}
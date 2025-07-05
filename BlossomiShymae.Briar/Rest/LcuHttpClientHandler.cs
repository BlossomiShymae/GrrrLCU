using BlossomiShymae.Briar.Utils;

namespace BlossomiShymae.Briar.Rest
{
    internal class LcuHttpClientHandler : HttpClientHandler
    {
        public ProcessInfo? ProcessInfo { get; internal set; } = null;

        public RiotAuthentication? RiotAuthentication => ProcessInfo == null ?
            null : new(ProcessInfo.RemotingAuthToken);

        public string? BaseAddress => ProcessInfo == null ?
            null : $"https://127.0.0.1:{ProcessInfo.AppPort}";

        private Lazy<bool> _isFirstRequest = new(() => true);

        private Lazy<bool> _isFailing = new(() => false);

        internal LcuHttpClientHandler() : base()
        {
            ServerCertificateCustomValidationCallback = DangerousAcceptAnyServerCertificateValidator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (_isFirstRequest.Value)
                {
                    _isFirstRequest = new(() => false);
                    SetProcessInfo();
                }
                if (_isFailing.Value)
                {
                    _isFailing = new(() => false);
                    SetProcessInfo();
                }
                PrepareRequestMessage(request);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                return response;
            }
            catch (InvalidOperationException)
            {
                _isFailing = new(() => true);
                throw;
            }
            catch (HttpRequestException)
            {
                try
                {
                    SetProcessInfo();
                    PrepareRequestMessage(request);
                    var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    return response;
                }
                catch (InvalidOperationException)
                {
                    _isFailing = new(() => true);
                    throw;
                }
            }
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (_isFirstRequest.Value)
                {
                    _isFirstRequest = new(() => false);
                    SetProcessInfo();
                }
                if (_isFailing.Value)
                {
                    _isFailing = new(() => false);
                    SetProcessInfo();
                }
                PrepareRequestMessage(request);
                return base.Send(request, cancellationToken);
            }
            catch (InvalidOperationException)
            {
                _isFailing = new(() => true);
                throw;
            }
            catch (HttpRequestException)
            {
                try
                {
                    SetProcessInfo();
                    PrepareRequestMessage(request);
                    return base.Send(request, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    _isFailing = new(() => true);
                    throw;
                }
            }
        }

        private void SetProcessInfo()
        {
            ProcessInfo = ProcessFinder.GetProcessInfo();
            if (!ProcessFinder.IsPortOpen(ProcessInfo))
                throw new InvalidOperationException("Failed to connect to LCUx process port.");
        }

        private void PrepareRequestMessage(HttpRequestMessage request)
        {       
            if (BaseAddress != null)
            {
                var requestUri = new Uri($"{BaseAddress}{request.RequestUri?.PathAndQuery ?? "/"}");
                request.RequestUri = requestUri;
            }
            request.Headers.Authorization = RiotAuthentication?.ToAuthenticationHeaderValue();
        }
    }
}
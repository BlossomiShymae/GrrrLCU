using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace BlossomiShymae.GrrrLCU
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

        private X509Certificate2 _certificate;

        internal LcuHttpClientHandler() : base()
        {
            _certificate = GetCertificate();

            ServerCertificateCustomValidationCallback = (_, certificate2, _, _) =>
            {
                var chain = new X509Chain();
                chain.ChainPolicy.ExtraStore.Add(_certificate);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                var isValid = certificate2 != null && chain.Build(certificate2);
                var chainRoot = chain.ChainElements[^1].Certificate;

                isValid = isValid && chainRoot.RawData.SequenceEqual(_certificate.RawData);

                return isValid;
            };
        }

        private static X509Certificate2 GetCertificate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BlossomiShymae.GrrrLCU.Assets.riotgames.pem";

            var stream = assembly.GetManifestResourceStream(resourceName);
            var memoryStream = new MemoryStream();
            stream!.CopyTo(memoryStream);

            return new X509Certificate2(memoryStream.ToArray());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (_isFirstRequest.Value)
                {
                    _isFirstRequest = new(() => true);
                    ProcessInfo = ProcessFinder.Get();
                }
                if (_isFailing.Value)
                {
                    _isFailing = new(() => false);
                    ProcessInfo = ProcessFinder.Get();
                }

                PrepareRequestMessage(request);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                return response;
            }
            catch (HttpRequestException)
            {
                try
                {
                    ProcessInfo = ProcessFinder.Get();

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
                    _isFirstRequest = new(() => true);
                    ProcessInfo = ProcessFinder.Get();
                }
                if (_isFailing.Value)
                {
                    _isFailing = new(() => false);
                    ProcessInfo = ProcessFinder.Get();
                }

                PrepareRequestMessage(request);

                return base.Send(request, cancellationToken);
            }
            catch (HttpRequestException)
            {
                try
                {
                    ProcessInfo = ProcessFinder.Get();

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

        private void PrepareRequestMessage(HttpRequestMessage request)
        {       
            if (BaseAddress != null)
            {
                request.RequestUri = new Uri($"{request.RequestUri?.ToString().Replace("https://127.0.0.1", BaseAddress)}");
                
                try
                {   
                    using var tcpClient = new TcpClient();
                    tcpClient.Connect("127.0.0.1", ProcessInfo!.AppPort);
                    tcpClient.Close();
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Failed to connect to LCUx process port.");
                }
                
            }
            request.Headers.Authorization = RiotAuthentication?.ToAuthenticationHeaderValue();
        }
    }
}
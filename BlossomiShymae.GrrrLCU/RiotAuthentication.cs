using System.Net.Http.Headers;
using System.Text;

namespace BlossomiShymae.GrrrLCU
{
    internal class RiotAuthentication(string RemotingAuthToken)
    {
        public string Password { get; } = RemotingAuthToken;
        public string RawValue { get => $"riot:{Password}"; }
        public string Value { get => Convert.ToBase64String(Encoding.UTF8.GetBytes(RawValue)); }

        public AuthenticationHeaderValue ToAuthenticationHeaderValue()
        {
            return new AuthenticationHeaderValue("Basic", Value);
        }
    }
}
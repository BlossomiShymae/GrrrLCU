using System.Net.Http.Headers;
using System.Text;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Repesents authentication for the League Client.
    /// </summary>
    /// <param name="RemotingAuthToken"></param>
    public class RiotAuthentication(string RemotingAuthToken)
    {
        /// <summary>
        /// Password component of the authentication.
        /// </summary>
        public string Password { get; } = RemotingAuthToken;
        /// <summary>
        /// Authentication value before Base64 conversion.
        /// </summary>
        public string RawValue { get => $"riot:{Password}"; }
        /// <summary>
        /// Authentication value in Base64 format.
        /// </summary>
        public string Value { get => Convert.ToBase64String(Encoding.UTF8.GetBytes(RawValue)); }

        /// <summary>
        /// Get an AuthenticationHeaderValue instance.
        /// </summary>
        /// <returns></returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue()
        {
            return new AuthenticationHeaderValue("Basic", Value);
        }
    }
}
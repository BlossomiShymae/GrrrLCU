using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using BlossomiShymae.Briar.Utils.Behaviors;
using Gapotchenko.FX.Diagnostics;

namespace BlossomiShymae.Briar.Utils
{
    /// <summary>
    /// Represents process information of the League Client.
    /// </summary>
    public class ProcessInfo
    {
        /// <summary>
        /// The authentication token for the LCU.
        /// </summary>
        public string RemotingAuthToken { get; }

        /// <summary>
        /// The port used for the LCU.
        /// </summary>
        public int AppPort { get; }

        private static readonly List<IPortTokenBehavior> _behaviors =
        [
            new PortTokenWithWin32Native(),
            new PortTokenWithLockfile(),
            new PortTokenWithGapotechnko()
        ];

        internal ProcessInfo(Process process)
        {
            var exceptions = new List<Exception>();
            foreach (var behavior in _behaviors)
            {
                if (behavior.TryGet(process, out var token, out var appPort, out var exception))
                {
                    RemotingAuthToken = token;
                    AppPort = appPort;
                    return;
                }

                exceptions.Add(exception);
            }

            throw new AggregateException("Unable to obtain process information.", exceptions);
        }
    }
}
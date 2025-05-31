using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BlossomiShymae.Briar.Utils.Behaviors
{
    /// <summary>
    /// A behavioral contract to provide a token and port from a process.
    /// </summary>
    internal interface IPortTokenBehavior
    {
        /// <summary>
        /// Attempt to get the token and port of a League Client process.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="remotingAuthToken"></param>
        /// <param name="appPort"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryGet(Process process, out string remotingAuthToken, out int appPort, [NotNullWhen(false)] out Exception? exception);

        /// <summary>
        /// Maps a list of command line arguments to a dictionary by argument name.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="_args"></param>
        void MapArguments(IReadOnlyList<string> args, Dictionary<string, string> _args)
        {
            foreach (var arg in args)
            {
                if (arg.Length > 0 && arg.Contains('='))
                {
                    var split = arg[2..].Split("=");
                    var key = split[0];
                    var value = split[1];

                    _args[key] = value;
                }
            }
        }
    }
}
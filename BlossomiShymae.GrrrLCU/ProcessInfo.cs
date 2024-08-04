using System.Diagnostics;

namespace BlossomiShymae.GrrrLCU
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

        private Dictionary<string, string> _args { get; set; } = [];

        internal ProcessInfo(Process process)
        {
            var rc = ProcessCommandLine.Retrieve(process, out var cl);
            if (rc == 0)
            {
                var args = ProcessCommandLine.CommandLineToArgs(cl);
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

                RemotingAuthToken = _args["remoting-auth-token"];
                AppPort = int.Parse(_args["app-port"]);
            } 
            else
            { 
                throw new InvalidOperationException("Failed to retrieve process command line."); 
            }
        }
    }
}
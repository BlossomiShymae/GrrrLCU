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
            if (TryGetCommandLineArguments(process, out var token, out var port, out var commandException))
            {
                RemotingAuthToken = token;
                AppPort = port;
            }
            else if (TryGetLockfile(process, out var _token, out var _port, out var lockException))
            {
                RemotingAuthToken = _token;
                AppPort = _port;
            }
            else
            {
                throw new AggregateException("Unable to obtain process information.", [commandException!, lockException!]);
            }
        }

        private bool TryGetLockfile(Process process, out string token, out int port, out Exception? ex)
        {
            try
            {
                var path = Directory.GetParent(process.MainModule!.FileName)!.FullName;
                var lockfilePath = Path.Join(path, "lockfile");

                using var lockfileStream = File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.Write);
                using var lockfileReader = new StreamReader(lockfileStream);

                var lockfile = lockfileReader.ReadToEnd();
                var values = lockfile.Split(":");

                token = values[3];
                port = int.Parse(values[2]);
                ex = null;
                return true;
            }
            catch (Exception e)
            {
                token = string.Empty;
                port = 0;
                ex = e;
                return false;
            }
        }

        private bool TryGetCommandLineArguments(Process process, out string remotingAuthToken, out int appPort, out Exception? ex)
        {
            try
            {
                var args = process.GetCommandLineArguments();
    
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
    
                remotingAuthToken = _args["remoting-auth-token"];
                appPort = int.Parse(_args["app-port"]);
                ex = null;
                return true;
            }
            catch (Exception e)
            {
                remotingAuthToken = string.Empty;
                appPort = 0;
                ex = e;
                return false;
            }
        }
    }
}
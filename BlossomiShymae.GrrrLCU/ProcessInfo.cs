using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
            // Fastest, more intrusive
            if (TryGetCommandLineArgumentsByWin32Native(process, out var t_native, out var ap_native, out var nativeException))
            {
                RemotingAuthToken = t_native;
                AppPort = ap_native;
            }
            else if (TryGetLockfile(process, out var t_lock, out var ap_lock, out var lockException))
            {
                RemotingAuthToken = t_lock;
                AppPort = ap_lock;
            }
            // Slowest, least intrusive
            else if (TryGetCommandLineArgumentsByWMI(process, out var t_wmi, out var ap_wmi, out var commandException))
            {
                RemotingAuthToken = t_wmi;
                AppPort = ap_wmi;
            }
            else
            {
                throw new AggregateException("Unable to obtain process information.", [nativeException, lockException, commandException]);
            }
        }

        private bool TryGetLockfile(Process process, out string token, out int port, [NotNullWhen(false)] out Exception? ex)
        {
            try
            {
                var path = Path.GetDirectoryName(process.MainModule!.FileName.AsSpan());
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

        private bool TryGetCommandLineArgumentsByWMI(Process process, out string remotingAuthToken, out int appPort, [NotNullWhen(false)] out Exception? ex)
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
                _args.Clear();
                return false;
            }
        }

        private bool TryGetCommandLineArgumentsByWin32Native(Process process, out string remotingAuthToken, out int appPort, [NotNullWhen(false)] out Exception? ex)
        {
            try
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

                    remotingAuthToken = _args["remoting-auth-token"];
                    appPort = int.Parse(_args["app-port"]);
                    ex = null;
                    return true;
                } 
                else throw new InvalidOperationException(ProcessCommandLine.ErrorToString(rc)); 
            }
            catch (Exception e)
            {
                remotingAuthToken = string.Empty;
                appPort = 0;
                ex = e ;
                _args.Clear();
                return false;
            }
        }
    }
}
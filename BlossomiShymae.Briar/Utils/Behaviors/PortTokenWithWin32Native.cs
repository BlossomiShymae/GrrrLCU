using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using BlossomiShymae.Briar.Utils.Windows;

[assembly:InternalsVisibleTo("BlossomiShymae.Briar.Benchmarks")]
namespace BlossomiShymae.Briar.Utils.Behaviors
{
    /// <summary>
    /// A behavioral port token class that uses native Win32 methodss.
    /// </summary>
    internal class PortTokenWithWin32Native : IPortTokenBehavior
    {
        /// <summary>
        /// Attempt to get the token and port of a League Client process using native Win32 methods.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="remotingAuthToken"></param>
        /// <param name="appPort"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryGet(Process process, out string remotingAuthToken, out int appPort, [NotNullWhen(false)] out Exception? exception)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    throw new PlatformNotSupportedException("This behavior is only supported on Windows");
                }
                var rc = ProcessCommandLine.Retrieve(process, out var cl);
                if (rc == 0)
                {
                    var args = ProcessCommandLine.CommandLineToArgs(cl);
                    var _args = new Dictionary<string, string>();
                    ((IPortTokenBehavior)this).MapArguments(args, _args);

                    remotingAuthToken = _args["remoting-auth-token"];
                    appPort = int.Parse(_args["app-port"]);
                    exception = null;
                    return true;
                }
                else throw new InvalidOperationException(ProcessCommandLine.ErrorToString(rc));
            }
            catch (Exception e)
            {
                remotingAuthToken = string.Empty;
                appPort = 0;
                exception = e;
                return false;
            }
        }
    }
}
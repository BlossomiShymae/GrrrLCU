using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("BlossomiShymae.Briar.Benchmarks")]
[assembly: InternalsVisibleTo("BlossomiShymae.Briar.Tests")]
namespace BlossomiShymae.Briar.Utils.Behaviors
{
    /// <summary>
    /// A behavioral port token class that uses the process list.
    /// </summary>
    internal class PortTokenWithProcessList : IPortTokenBehavior
    {
        /// <summary>
        /// Attempt to get the token and port of a League Client process from the process list.
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
                var tokenRegex = @"--remoting-auth-token=([\w-]*)";
                var portRegex = @"--app-port=([0-9]*)";
                ProcessStartInfo processStartInfo;
                if (OperatingSystem.IsWindows())
                {
                    processStartInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = "Get-CimInstance Win32_Process -Filter \"\"\"name = 'LeagueClientUx.exe'\"\"\" | Select-Object -ExpandProperty CommandLine",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                }
                else if (OperatingSystem.IsMacOS())
                {
                    processStartInfo = new ProcessStartInfo()
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"ps x -o args | grep 'LeagueClientUx'\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                }
                else
                {
                    throw new PlatformNotSupportedException("This behavior is only supported on Windows or MacOS.");
                }

                using var subprocess = new Process()
                {
                    StartInfo = processStartInfo
                };
                subprocess.Start();
                subprocess.WaitForExit();
                var output = subprocess.StandardOutput.ReadToEnd();
                var error = subprocess.StandardError.ReadToEnd();
                if (string.IsNullOrEmpty(output))
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }
                    throw new FormatException("The output string is empty.");
                }

                remotingAuthToken = Regex.Match(output, tokenRegex).Groups[1].Value;
                appPort = int.Parse(Regex.Match(output, portRegex).Groups[1].Value);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                remotingAuthToken = string.Empty;
                appPort = 0;
                exception = ex;
                return false;
            }
        }
    }
}
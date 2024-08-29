using System.Diagnostics;
using System.Net.Sockets;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// A helper class to find the League Client process.
    /// </summary>
    public static class ProcessFinder
    {
        /// <summary>
        /// Get the current running League Client process.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Process GetProcess()
        {
            Process? process = null;

            foreach (var _process in Process.GetProcessesByName("LeagueClientUx"))
            {
                switch(_process.ProcessName)
                {
                    case "LeagueClientUx":
                        process = _process;
                        break;
                    default:
                        break;
                }

                if (process != null) break;
            }

            return process ?? throw new InvalidOperationException("Failed to find LCUx process.");
        }
        /// <summary>
        /// Get the current running League Client process information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ProcessInfo GetProcessInfo()
        {
            return new ProcessInfo(GetProcess());
        }

        /// <summary>
        /// Get the current process status of the League Client. This does not 
        /// necessarily mean that the League Client can be connected to.
        /// In that case, use <see cref="IsPortOpen()"/>.
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            try
            {
                GetProcessInfo();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the current port status of the League Client process.
        /// </summary>
        /// <returns></returns>
        public static bool IsPortOpen()
        {
            try
            {
                var processInfo = GetProcessInfo();
                return IsPortOpen(processInfo);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the current port status of the League Client process.
        /// </summary>
        /// <param name="processInfo"></param>
        /// <returns></returns>
        public static bool IsPortOpen(ProcessInfo processInfo)
        {
            try
            {
                using var tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", processInfo.AppPort);
                tcpClient.Close();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
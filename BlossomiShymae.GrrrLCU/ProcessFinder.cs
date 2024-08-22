using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// A helper class to find the League Client process.
    /// </summary>
    public static class ProcessFinder
    {
        /// <summary>
        /// Get the current running League Client process information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ProcessInfo Get()
        {
            ProcessInfo? processInfo = null;

            foreach (var process in Process.GetProcessesByName("LeagueClientUx"))
            {
                switch(process.ProcessName)
                {
                    case "LeagueClientUx":
                        processInfo = new ProcessInfo(process);
                        break;
                    default:
                        break;
                }

                if (processInfo != null) break;
            }

            return processInfo ?? throw new InvalidOperationException("Failed to find LCUx process.");
        }

        /// <summary>
        /// Get the current process status of the League Client. This does not 
        /// necessarily mean that the League Client can be connected to.
        /// In that case, use <see cref="IsPortOpen"/>.
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            try
            {
                Get();
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
                var processInfo = Get();
                using var tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", processInfo.AppPort);
                tcpClient.Close();
                return true;
                
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
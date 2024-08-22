using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /// Get the current active status of the League Client process;
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
    }
}
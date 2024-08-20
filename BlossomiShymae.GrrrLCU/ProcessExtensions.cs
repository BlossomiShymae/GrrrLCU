using System.Diagnostics;
using System.Management;

namespace BlossomiShymae.GrrrLCU
{
    internal static class ProcessExtensions
    {
        internal static string[] GetCommandLineArguments(this Process process)
        {
            var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
            var objects = searcher.Get();
            var commandLine = objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            return commandLine!
                .Split(" ")
                .Select(arg => arg.Replace("\"", string.Empty))
                .ToArray();
        }
    }
}
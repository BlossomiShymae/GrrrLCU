using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlossomiShymae.Briar.Benchmarks")]
namespace BlossomiShymae.Briar.Utils.Behaviors
{
    /// <summary>
    /// A behaviorial port token class that uses the lockfile.
    /// </summary>
    internal class PortTokenWithLockfile : IPortTokenBehavior
    {
        /// <summary>
        /// Attempt to get the token and port of a League Client process from the lockfile.
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
                string path = Path.GetDirectoryName(process.MainModule!.FileName)!;
                string lockfilePath = Path.Join(path, "lockfile");
                while (!File.Exists(lockfilePath) || Path.GetPathRoot(path) != path)
                {
                    if (Directory.GetParent(path) is DirectoryInfo parent)
                    {
                        path = parent.FullName;
                    }
                    else
                    {
                        path = Path.GetPathRoot(path)!;
                    }
                    lockfilePath = Path.Join(path, "lockfile");
                    
                }
                using var lockfileStream = File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.Write);
                using var lockfileReader = new StreamReader(lockfileStream);

                var lockfile = lockfileReader.ReadToEnd();
                var values = lockfile.Split(":");

                remotingAuthToken = values[3];
                appPort = int.Parse(values[2]);
                exception = null;
                return true;
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
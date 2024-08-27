using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Gapotchenko.FX.Diagnostics;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// A behavioral port token class that uses the Gapotechnko package.
    /// </summary>
    public class PortTokenWithGapotechnko : IPortTokenBehavior
    {
        /// <summary>
        /// Attempt to get the token and port of a League Client process with the Gapotechnko package.
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
                var args = process.ReadArgumentList();
                var _args = new Dictionary<string, string>();
                ((IPortTokenBehavior)this).MapArguments((IReadOnlyList<string>)args, _args);

                remotingAuthToken = _args["remoting-auth-token"];
                appPort = int.Parse(_args["app-port"]);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Helpers
{
    public static class PosHelper
    {
        public static bool PingHost(string nameOrAddress)
        {
            var pingable = false;

            try
            {
                using (var pinger = new Ping())
                {
                    var reply = pinger.Send(nameOrAddress);
                    pingable = reply?.Status == IPStatus.Success;
                }
            }
            catch (PingException)
            {
            }

            return pingable;
        }
    }
}

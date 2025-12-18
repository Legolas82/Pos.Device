using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using NLog;
using Topshelf;
using Topshelf.Runtime;

namespace POS.Device.Web.API
{
    public class StartupService : ServiceControl
    {
        private readonly HostSettings _hostSettings;
        private readonly string[] _args;
        private readonly string _contentRoot;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IDisposable _webApp;

        public StartupService(string contentRoot)
        {
            _contentRoot = contentRoot;
        }

        public StartupService(string[] args)
        {
            _args = args;
        }

        public StartupService(HostSettings hostSettings)
        {
            _hostSettings = hostSettings;
            _args = new string[]{};
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                var t = new Thread(Listener);
                t.Start();
                //Listener();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                //throw;
            }

            return true;

        }

        private void Listener()
        {
            try
            {
                var host = Properties.Settings.Default.Host;
                var port = Properties.Settings.Default.Port;
                var url = $"http://{host}:{port}/";

                logger.Debug($"Init web service {url}");
                Console.WriteLine($"Init web service {url}");
                var baseAddress = $"{url}";
                _webApp = WebApp.Start<StartUp>(url: baseAddress);
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }

        public bool Stop(HostControl hostControl)
        {
            Console.WriteLine("Stop Pax web service");
            _webApp.Dispose();
            LogManager.Shutdown();
            return true;
        }

    }
}

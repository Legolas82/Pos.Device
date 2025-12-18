using System;
using Microsoft.Owin.Hosting;
using Topshelf;

namespace POS.Device.Web.API
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTopshelf(args);
        }

        static void StartTopshelf(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<StartupService>(s =>
                {
                    s.ConstructUsing(settings => new StartupService(settings));
                    s.WhenStarted((tc, hostControl) => tc.Start(hostControl));
                    s.WhenStopped((tc, hostControl) => tc.Stop(hostControl));
                });
                x.RunAsLocalSystem();

                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(1); // restart the service after 1 minute
                });

                x.SetDescription("This is a Devices Web Service for DATASYS POS System.");
                x.SetDisplayName("Datasys Devices Web Service");
                x.SetServiceName("DatasysDevicesService");

                x.UseNLog();
            });
        }
    }
}
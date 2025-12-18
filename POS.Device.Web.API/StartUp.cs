using System;
using System.Web.Http;
using Owin;
using POS.Core.ViewModels;
using POS.ExternalMedia.Devices;
using POS.ExternalMedia.Interfaces;
using Unity;
using Unity.AspNet.WebApi;

namespace POS.Device.Web.API
{
    public class StartUp
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.Container);
            UnityConfig.Container.Resolve<IDeviceOutputManager<ScannerViewModel>>().InitDevice();
            UnityConfig.Container.Resolve<IDeviceOutputManager<ScaleViewModel>>().InitDevice();

            appBuilder.UseWebApi(config);

        }
    }
}
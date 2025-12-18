using System;
using POS.Core.ViewModels;
using POS.ExternalMedia;
using POS.ExternalMedia.Devices;
using POS.ExternalMedia.Enums;
using POS.ExternalMedia.Interfaces;
using Unity;
using Unity.Lifetime;

namespace POS.Device.Web.API
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            container.RegisterType<IPosDeviceController, PosDeviceController>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDeviceOutputManager<ScannerViewModel>, ScannerOposDevice>(new ContainerControlledLifetimeManager());

            var propScaleConnector = Properties.Settings.Default.ScaleConnector;
            Enum.TryParse(propScaleConnector, true, out EConnectorType connectorType);
            switch (connectorType)
            {
                case EConnectorType.OPOS:
                    container.RegisterType<IDeviceOutputManager<ScaleViewModel>, ScaleOposDevice>(new ContainerControlledLifetimeManager());
                    break;
                case EConnectorType.COM:
                    container.RegisterType<IDeviceOutputManager<ScaleViewModel>, ScaleComDevice>(new ContainerControlledLifetimeManager());
                    break;
                default:
                    container.RegisterType<IDeviceOutputManager<ScaleViewModel>, ScaleOposDevice>(new ContainerControlledLifetimeManager());
                    break;
            };
            
        }
    }
}
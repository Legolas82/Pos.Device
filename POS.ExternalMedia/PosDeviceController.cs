using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PointOfService;
using NLog;
using POS.ExternalMedia.Interfaces;
using Logger = NLog.Logger;

namespace POS.ExternalMedia
{
    public class PosDeviceController : IPosDeviceController, IDisposable
    {
        private readonly PosExplorer _posExplorer;
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public PosDeviceController()
        {
            _posExplorer = new PosExplorer();
        }

        public List<DeviceInfo> GetAll()
        {
            var devicesList = new List<DeviceInfo>();
            var devices = _posExplorer.GetDevices(); //DeviceType.CashDrawer

            // Iterate through the list looking for the one
            // needed for this sample.
            foreach (DeviceInfo d in devices)
            {
                devicesList.Add(d);
            }

            return devicesList;
        }

        public List<DeviceInfo> GetByType(string deviceType)
        {
            var devicesList = new List<DeviceInfo>();
            var devices = _posExplorer.GetDevices(deviceType); //DeviceType.CashDrawer

            // Iterate through the list looking for the one
            // needed for this sample.
            foreach (DeviceInfo d in devices)
            {
                devicesList.Add(d);
            }

            return devicesList;
        }

        public PosCommon OpenDevice(string deviceType)
        {
            DeviceInfo device = null;

            // Retrieve the list of PosPrinter Service Objects.
            var devices = _posExplorer.GetDevices(deviceType); //DeviceType.CashDrawer

            // Iterate through the list looking for the one
            // needed for this sample.
            foreach (DeviceInfo d in devices)
            {
                device = d;
                break;
            }

            if (device == null)
            {
                throw new Exception("Service Object not found");
            }

            var posCommon = (PosCommon)_posExplorer.CreateInstance(device);
            posCommon.Open();
            posCommon.Claim(2000);
            posCommon.DeviceEnabled = true;

            Console.WriteLine($"Device {device.Type} {device.Description} open");
            logger.Info($"Device {device.Type} {device.Description} open");

            return posCommon;
        }

        public PosCommon OpenDevice(DeviceInfo device)
        {
            // Retrieve the list of PosPrinter Service Objects.
            if (device == null)
            {
                throw new Exception("Service Object not found");
            }

            try
            {
                var posCommon = (PosCommon)_posExplorer.CreateInstance(device);
                posCommon.Open();
                posCommon.Claim(2000);
                posCommon.DeviceEnabled = true;

                Console.WriteLine($"Device {device.Type} {device.Description} open");
                logger.Info($"Device {device.Type} {device.Description} open");

                return posCommon;
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }

            Console.WriteLine($"Error opening device {device.Description}");
            logger.Error($"Error opening device {device.Description}");

            return null;

        }

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {

        }

        #endregion
    }
}

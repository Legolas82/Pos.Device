using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PointOfService;
using NLog;
using POS.Core.Client;
using POS.Core.ViewModels;
using POS.ExternalMedia.Enums;
using POS.ExternalMedia.Interfaces;
using Logger = NLog.Logger;

namespace POS.ExternalMedia.Devices
{
    public class ScannerOposDevice : IDeviceOutputManager<ScannerViewModel>, IDisposable
    {
        private readonly IPosDeviceController _deviceController;
        private readonly string _deviceName;
        private PosCommon _deviceCommon;
        private readonly ScannerViewModel _scannerViewModel;
        private readonly string _posWebApi;

        private Logger logger = LogManager.GetCurrentClassLogger();

        public ScannerOposDevice(IPosDeviceController deviceController)
        {
           _deviceController = deviceController;

           var prop = Properties.Settings.Default;
           _deviceName = prop.ScannerConnection;
           _posWebApi = prop.PosWebApiUri;

            _scannerViewModel = new ScannerViewModel();
        }

        public void InitDevice(){
            try
            {
                var devices = _deviceController.GetByType(DeviceType.Scanner);
                var device = devices.FirstOrDefault(d => d.ServiceObjectName == _deviceName);
                if (device == null)
                    throw new Exception($"Scanner not found by name {_deviceName}.");

                _deviceCommon = _deviceController.OpenDevice(device);
                if (_deviceCommon != null)
                {
                    ((Scanner)_deviceCommon).DataEventEnabled = true;
                    ((Scanner)_deviceCommon).DecodeData = true;
                    ((Scanner)_deviceCommon).AutoDisable = false;
                    ((Scanner)_deviceCommon).DataEvent += OnDataEvent;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }

        public void OpenDevice()
        {
            if (_deviceCommon != null)
            {
                if (_deviceCommon.State == ControlState.Closed) _deviceCommon.Open();
                _deviceCommon.DeviceEnabled = true;
                ((Scanner)_deviceCommon).DataEventEnabled = true;
                ((Scanner)_deviceCommon).DecodeData = true;
                logger.Info("Enable scanner");
            }
        }

        public void CloseDevice()
        {
            if (_deviceCommon != null)
            {
                try
                {
                    if (_deviceCommon.Claimed)
                    {
                        _deviceCommon.Release();
                        _deviceCommon.Close();
                        logger.Info("Disable Scanner success.");
                    }
                    else
                    {
                        logger.Error("Cannot Disable Scanner. Not Claimed.");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Cannot Disable Scanner. Unhandler Exception.");
                    logger.Error(e.Message);
                }
            } else
            {
                logger.Error("Cannot Disable Scanner. Not Open.");
            }
        }

        public EConnectorType Type() => EConnectorType.OPOS;

        public ScannerViewModel GetDeviceValue(){
            return _scannerViewModel;
        }

        private async void OnDataEvent(object sender, DataEventArgs e)
        {
            var b = ((Scanner)_deviceCommon).ScanData;

            var str = b.Aggregate("Raw Data: ", (current, t) => current + (t.ToString(System.Globalization.CultureInfo.CurrentCulture) + " "));
            str += "\r\n";

            str += "Formatted Data: ";
            b = ((Scanner)_deviceCommon).ScanDataLabel;
            str = b.Aggregate(str, (current, t) => current + (char) t);
            str += "\r\n";

            str += "Symbology: " + ((Scanner)_deviceCommon).ScanDataType + "\r\n";
            str += "\r\n";

            logger.Debug(str);

            _scannerViewModel.Data = Encoding.Default.GetString(((Scanner)_deviceCommon).ScanDataLabel);
            _scannerViewModel.Type = ((Scanner) _deviceCommon).ScanDataType.ToString();

            try
            {
                var dataEventEnabled = _deviceCommon.GetType().GetProperty("DataEventEnabled");
                dataEventEnabled?.SetValue(_deviceCommon, true, null);
            }
            catch (Exception exception)
            {
                logger.Error(exception, exception.Message);
            }

            try
            {
                await WebAPIHelper.Post(_posWebApi, "/api/1.0/Devices/scanner", _scannerViewModel);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }

        }

        public void Dispose()
        {
            CloseDevice();
        }
    }
}

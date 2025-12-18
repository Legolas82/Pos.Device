using System;
using System.Linq;
using Microsoft.PointOfService;
using NLog;
using POS.Core.Client;
using POS.Core.Exceptions;
using POS.Core.ViewModels;
using POS.ExternalMedia.Enums;
using POS.ExternalMedia.Interfaces;
using Logger = NLog.Logger;

namespace POS.ExternalMedia.Devices
{
    public class ScaleOposDevice : IDeviceOutputManager<ScaleViewModel>, IDisposable
    {
        private const int Timeout = 3000;
        private readonly IPosDeviceController _deviceController;
        private readonly string _deviceName;
        private PosCommon _deviceCommon;
        private readonly ScaleViewModel _scaleViewModel;
        private readonly string _posWebApi;

        private Logger logger = LogManager.GetCurrentClassLogger();

        public ScaleOposDevice(IPosDeviceController deviceController)
        {
           _deviceController = deviceController;

           var prop = Properties.Settings.Default;
           _deviceName = prop.ScaleConnection;
           _posWebApi = prop.PosWebApiUri;

           _scaleViewModel = new ScaleViewModel();
        }

        public void InitDevice(){
            try
            {
                var devices = _deviceController.GetByType(DeviceType.Scale);
                var device = devices.FirstOrDefault(d => d.ServiceObjectName == _deviceName);
                if (device == null)
                    throw new Exception($"Scale not found by name {_deviceName}.");

                _deviceCommon = _deviceController.OpenDevice(device);
                if (_deviceCommon != null)
                {
                    ((Scale)_deviceCommon).DataEventEnabled = true;
                    ((Scale)_deviceCommon).AutoDisable = true;
                    ((Scale)_deviceCommon).DataEvent += OnDataEvent;
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
                ((Scale)_deviceCommon).DataEventEnabled = true;
                logger.Info("Enable scale");
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
                        logger.Info("Disable Scale success.");
                    }
                    else
                    {
                        logger.Error("Cannot Disable Scale. Not Claimed.");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Cannot Disable Scale. Unhandler Exception.");
                    logger.Error(e.Message);
                }
            }
            else
            {
                logger.Error("Cannot Disable Scale. Not Open.");
            }
        }

        public EConnectorType Type() => EConnectorType.OPOS;

        public ScaleViewModel GetDeviceValue()
        {
            if(_deviceCommon == null) throw new Exception("Scale device not ready.");

            _deviceCommon.DeviceEnabled = true;
            ((Scale)_deviceCommon).DataEventEnabled = true;

            try
            {
                var weight = ((Scale) _deviceCommon).ReadWeight(Timeout);
                _scaleViewModel.Weight = weight;
                return _scaleViewModel;
            }
            catch (PosControlException e)
            {
                logger.Error(e, e.Message);
                if (e.ErrorCode == ErrorCode.Failure)
                {
                    throw new PosScaleReadWeightException("Place a product on the scale.");
                }
                throw;
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
                throw;
            }
        }

        private async void OnDataEvent(object sender, DataEventArgs e)
        {
            var str = $"Item Weight: {_scaleViewModel.Weight}" ;

            logger.Info(str);

            try
            {
                await WebAPIHelper.Post(_posWebApi, "/api/1.0/Devices/Scale", _scaleViewModel);
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

using System;
using System.IO.Ports;
using System.Threading;
using NLog;
using POS.Core.Exceptions;
using POS.Core.ViewModels;
using POS.ExternalMedia.Enums;
using POS.ExternalMedia.Interfaces;

namespace POS.ExternalMedia.Devices
{
    public class ScaleComDevice : IDeviceOutputManager<ScaleViewModel>, IDisposable
    {
        private SerialPort _port;
        public string Value { get; set; }

        private Logger logger = LogManager.GetCurrentClassLogger();
        private string _posWebApi;
        private readonly string _comPort;

        public ScaleComDevice()
        {
            var prop = Properties.Settings.Default;
            _comPort = ValidateComPort(prop.ScaleConnection) ? prop.ScaleConnection : "COM1";
            _posWebApi = prop.PosWebApiUri;
        }

        public void InitDevice()
        {
            try
            {
                logger.Debug($"Init SerialPort {_comPort}");
                _port = new SerialPort(_comPort, 9600, Parity.None, 7, StopBits.One);

                logger.Debug($"Open SerialPort {_comPort}");
                _port.Open();

                logger.Debug("Start Data received");
                _port.DataReceived += PortOnDataReceived;

                //_port.ErrorReceived += PortOnErrorReceived;
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }

        public ScaleViewModel GetDeviceValue()
        {
            try
            {
                Value = string.Empty; // Clear value
                logger.Debug($"Write to {_comPort} port");
                _port.Write("W");

                Thread.Sleep(500);

                logger.Debug($"Value read from {_comPort} port: {Value}");
                if (!string.IsNullOrEmpty(Value))
                {
                    try
                    {
                        logger.Debug($"Convert value: {Value}");
                        var start = Value.IndexOfAny("0123456789".ToCharArray());
                        if (start > 0)
                        {
                            var substring = Value.Substring(start, 5);
                            var weight = Convert.ToDouble(substring);
                            logger.Debug($"Return value: {weight}");
                            return new ScaleViewModel
                            {
                                Weight = (decimal)(weight / 100)
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                logger.Error($"Error. Don't read {_comPort} data. Place a product on the scale.");
                throw new PosScaleReadWeightException("Place a product on the scale.");
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
                throw;
            }
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var length = _port.BytesToRead;
                logger.Debug($"Read {_comPort} port length: {length}");
                if (length >= 7)
                {
                    try
                    {
                        Value = _port.ReadExisting();
                        logger.Debug($"Read {_comPort} port value: {Value}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
        }

        private void PortOnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            var posScaleReadWeightException = new PosScaleReadWeightException(e.ToString());
            logger.Error(posScaleReadWeightException, e.EventType.ToString);
            throw posScaleReadWeightException;
        }

        private bool ValidateComPort(string comPort)
        {
            return comPort.Substring(0, 3).Equals("COM");
        }

        public void OpenDevice()
        {
        }

        public void CloseDevice()
        {
            try
            {
                _port.Close();
                _port.Dispose();
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
                throw;
            }
        }

        public EConnectorType Type() => EConnectorType.COM;

        public void Dispose()
        {
            CloseDevice();
        }
    }
}

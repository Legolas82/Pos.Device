using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PointOfService;

namespace POS.ExternalMedia.Interfaces
{
    public interface IPosDeviceController
    {
        List<DeviceInfo> GetAll();

        List<DeviceInfo> GetByType(string deviceType);

        PosCommon OpenDevice(string deviceType);

        PosCommon OpenDevice(DeviceInfo device);
    }
}

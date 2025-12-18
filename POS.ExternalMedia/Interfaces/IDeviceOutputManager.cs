using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ExternalMedia.Interfaces
{
    public interface IDeviceOutputManager<T> : IDeviceManager, IDeviceOutput<T>
    {
    }
}

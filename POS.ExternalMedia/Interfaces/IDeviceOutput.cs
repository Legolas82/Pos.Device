using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ExternalMedia.Interfaces
{
    public interface IDeviceOutput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T GetDeviceValue();
    }
}

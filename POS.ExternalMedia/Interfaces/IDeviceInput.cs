using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ExternalMedia.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeviceInput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void SetDeviceValue(T value);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.ExternalMedia.Enums;

namespace POS.ExternalMedia.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeviceManager
    {
        /// <summary>
        /// 
        /// </summary>
        void InitDevice();

        /// <summary>
        /// 
        /// </summary>
        void OpenDevice();

        /// <summary>
        /// 
        /// </summary>
        void CloseDevice();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        EConnectorType Type();

    }
}

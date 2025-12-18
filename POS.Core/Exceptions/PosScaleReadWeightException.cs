using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Exceptions
{
    public class PosScaleReadWeightException : Exception
    {
        /// <inheritdoc />
        public PosScaleReadWeightException(string message) : base(message)
        {
        }
    }
}

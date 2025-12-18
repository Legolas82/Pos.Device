using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Exceptions
{
    public class PosPaymentException : Exception
    {
        public string ResultCode { get; }
        public string ResultText { get; }
        /// <inheritdoc />
        public PosPaymentException()
        {
        }

        /// <inheritdoc />
        public PosPaymentException(string resultCode, string resultText)
            :this($"{resultText} - Code {resultCode}")
        {
            ResultCode = resultCode;
            ResultText = resultText;
        }

        /// <inheritdoc />
        public PosPaymentException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public PosPaymentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected PosPaymentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

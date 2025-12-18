using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.ExternalMedia.Devices;
using Xunit;
using FluentAssertions;
using POS.Core.Exceptions;

namespace POS.ExternalMedia.Test
{
    public class ScaleComDeviceTest
    {
        private readonly ScaleComDevice _scaleComDevice;

        public ScaleComDeviceTest()
        {
            _scaleComDevice = new ScaleComDevice();
            _scaleComDevice.InitDevice();
        }

        [Theory]
        [InlineData("?P00037", 0.37)]
        [InlineData("00037", 0.37)]
        [InlineData("01000", 10.00)]
        [InlineData("00945", 9.45)]
        [InlineData("01950", 19.50)]
        public void GetDeviceValueTest(string value, decimal expect)
        {
            _scaleComDevice.Value = value;
            var weight = _scaleComDevice.GetDeviceValue();
            weight.Weight.Should().Be(expect);
        }

        [Fact]
        public void GetDeviceValue_NotData_Test()
        {
            _scaleComDevice.Value = string.Empty;
            _scaleComDevice.Invoking(x => _scaleComDevice.GetDeviceValue()).Should().Throw<PosScaleReadWeightException>()
                .WithMessage("Place a product on the scale.");
        }
    }
}

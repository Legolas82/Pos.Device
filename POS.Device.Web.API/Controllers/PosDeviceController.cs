using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using NLog;
using POS.Core.Enums;
using POS.Core.Exceptions;
using POS.Core.ViewModels;
using POS.ExternalMedia.Devices;
using POS.ExternalMedia.Interfaces;

namespace POS.Device.Web.API.Controllers
{
    public class PosDeviceController : ApiController
    {
        private readonly IDeviceOutputManager<ScannerViewModel> _scanner;
        private readonly IDeviceOutputManager<ScaleViewModel> _scale;

        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public PosDeviceController(IDeviceOutputManager<ScannerViewModel> scanner, IDeviceOutputManager<ScaleViewModel> scale)
        {
            _scanner = scanner;
            _scale = scale;

        }

        [Route("api/PosDevice/scale/weight")]  
        public ScaleViewModel GetScale()
        {
            try
            {
                var result = _scale.GetDeviceValue();
                logger.Trace(result);

                return result;
            }
            catch (PosScaleReadWeightException e)
            {
                logger.Error(e, e.Message);
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(e.Message, Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(response);
            }
            catch (Exception e)                                                             
            {
                logger.Error(e, e.Message);
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.Message, Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(response);
            }
        }

        [Route("api/PosDevice")]
        public OkResult Post(EDeviceActions actions)
        {
            try
            {
                logger.Info($"Action: {actions}");
                switch (actions)
                {
                    case EDeviceActions.Open:
                        _scanner.OpenDevice();
                        _scale.OpenDevice();
                        break;
                    case EDeviceActions.Close:
                        _scanner.CloseDevice();
                        _scale.CloseDevice();
                        break;
                    case EDeviceActions.OpenScanner:
                        _scanner.OpenDevice();
                        break;
                    case EDeviceActions.CloseScanner:
                        _scanner.CloseDevice();
                        break;
                    case EDeviceActions.OpenScale:
                        _scale.OpenDevice();
                        break;
                    case EDeviceActions.CloseScale:
                        _scale.CloseDevice();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(actions), actions, null);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }

            return Ok();
        }

    }
}

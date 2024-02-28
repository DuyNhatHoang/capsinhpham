
using Data.MongoCollections;
using Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Core;

namespace Booking_Service_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FCMController : ControllerBase
    {
        private readonly IFCMService _service;

        public FCMController(IFCMService service)
        {
            _service = service;
        }

        [HttpPost("SetToken")]
        public async Task<IActionResult> SendNotify(CreateUserFirebaseToken token)
        {
            var result = await _service.SetFirebaseToken(token);
            return Ok(result);
        }

        [HttpPost("SendNotify")]
        public async Task<IActionResult> SendNotify(NotificationModel notificationModel)
        {
            var result = await _service.SendNotification(notificationModel);
            return Ok(result);
        }
        [HttpPost("SendNotifyForWeb")]
        public async Task<IActionResult> SendNotify4Web(WebNotificationModel notificationModel)
        {
            var result = await _service.SendNotificationForWeb(notificationModel);
            return Ok(result);
        }
    }
}

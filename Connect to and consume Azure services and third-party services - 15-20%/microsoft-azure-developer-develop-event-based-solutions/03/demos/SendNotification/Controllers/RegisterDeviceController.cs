using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.Azure.NotificationHubs;

namespace SendNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterDeviceController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(RegistrationRequest registration)
        {
            var pushService = new PushService();

            await pushService.RegisterDevice(registration);

            return new OkResult();
        }
    }
}
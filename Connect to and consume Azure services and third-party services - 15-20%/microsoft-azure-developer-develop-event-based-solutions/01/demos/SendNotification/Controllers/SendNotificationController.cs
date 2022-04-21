using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SendNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendNotificationController : ControllerBase
    {
        // from example at: https://github.com/Azure-Samples/azure-event-grid-viewer/blob/main/viewer/Controllers/UpdatesController.cs
        private bool EventTypeSubcriptionValidation
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "SubscriptionValidation";

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "Notification";

        private readonly ILogger<SendNotificationController> _logger;

        public SendNotificationController(ILogger<SendNotificationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var requestStream = new StreamReader(Request.Body))
            {
                var bodyJson = await requestStream.ReadToEndAsync();

                var events = JsonConvert.DeserializeObject<List<EventGridEvent>>(bodyJson);
                                            
                if (EventTypeSubcriptionValidation)
                {                
                    var subValidationEventData = 
                        ((JObject)events.First().Data).ToObject<SubscriptionValidationEventData>();
                                        
                    return new OkObjectResult(
                        new SubscriptionValidationResponse(subValidationEventData.ValidationCode)
                    );
                }
                else if (EventTypeNotification) 
                {                
                    var notificationEvent = events.First();

                    _logger.LogInformation(notificationEvent.Subject);

                    return new OkResult();
                }
            }

            return new BadRequestResult();
        }
    }
}

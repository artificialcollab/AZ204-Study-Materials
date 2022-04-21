using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.NotificationHubs;

namespace SendNotification
{
    public class PushService
    {
        NotificationHubClient _hubClient;
        readonly string _hubConnectionString = "";
        readonly string _hubName = "";

        public PushService()
        {
            _hubClient = new NotificationHubClient(_hubConnectionString, _hubName);            
        }

        public async Task SendPush(NewAccountMessage newAccountMessage)
        {            
            var applePayloadJson = "";
            var androidPayloadJson = "";
            
            // send specifically to firebase or apple devices
            await _hubClient.SendFcmNativeNotificationAsync(androidPayloadJson);
            await _hubClient.SendAppleNativeNotificationAsync(applePayloadJson);

            // send to all devices
            IDictionary<string,string> templateDictionary = 
                new Dictionary<string, string>();

            templateDictionary.Add("message", newAccountMessage.AccountName);

            // could send along template name as a tag
            await _hubClient.SendTemplateNotificationAsync(templateDictionary);            
        }

        public async Task RegisterDevice(RegistrationRequest registration)
        {
            var installation = new Installation {
                InstallationId = registration.InstallationId,                
                PushChannel = registration.PushChannel                
            };    
                    
            var installTemplate = new InstallationTemplate();            
            installTemplate.Body= "device specific json with variable placeholders";
            installation.Templates.Add("newaccount", installTemplate);     

            if (registration.PushChannel == "apns")
            {
                installation.Platform = NotificationPlatform.Apns;
            }
            else if (registration.PushChannel == "fcm")
            {
                installation.Platform = NotificationPlatform.Fcm;
            }
            else if (registration.PushChannel == "wns")
            {
                installation.Platform = NotificationPlatform.Wns;
            }
                        
            await _hubClient.CreateOrUpdateInstallationAsync(installation);
        }  
    }    
}
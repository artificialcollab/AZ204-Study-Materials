using System;


namespace SendNotification
{
    public class RegistrationRequest
    {        
        public string InstallationId { get; set; }
    
        public string Platform { get; set; }
        
        public string PushChannel { get; set; }
    }
}
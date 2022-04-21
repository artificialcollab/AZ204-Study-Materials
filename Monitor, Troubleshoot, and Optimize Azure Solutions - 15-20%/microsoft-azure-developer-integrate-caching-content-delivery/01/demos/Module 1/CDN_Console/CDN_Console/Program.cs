using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.Cdn;
using Microsoft.Azure.Management.Cdn.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace CDN_Console
{
    class Program
    {
        //Tenant app constants
        //You need to create an active directory app registration and configure a service principle
        //If you are unsure how to do this then follow the tutorial at: https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal
        private const string clientID = "{client ID}";
        private const string clientSecret = "{Client Secret}";
        private const string authority = "https://login.microsoftonline.com/{ActiveDirectory Tennant ID}/{Active Directory Tennant Domain}";

        //Application constants
        private const string subscriptionId = "{Azure subscription ID}";
        private const string profileName = "CDNConsoleProfile";
        private const string endpointName = "CDNProfileEndpoint";
        //Create a resource group and enter its name here
        private const string resourceGroupName = "{Resource Group Name}";
        private const string resourceLocation = "{Azure Region i.e westus}";

        private static bool profileAlreadyExists;
        private static bool endpointAlreadyExists;

        static void Main(string[] args)
        {
            AuthenticationResult authResult = GetAccessToken();


            CdnManagementClient cdn = new CdnManagementClient(new TokenCredentials(authResult.AccessToken))
            { SubscriptionId = subscriptionId };

            // Create a new CDN Profile
            CreateCdnProfile(cdn);

            // Create a new CDN Endpoint
            CreateCdnEndpoint(cdn);

            Console.ReadKey();

        }

        private static AuthenticationResult GetAccessToken()
        {
            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential credential = new ClientCredential(clientID, clientSecret);
            AuthenticationResult authResult =
                authContext.AcquireTokenAsync("https://management.core.windows.net/", credential).Result;

            return authResult;
        }

        private static void CreateCdnProfile(CdnManagementClient cdn)
        {            
                Console.WriteLine("Creating profile {0}.", profileName);
                
                cdn.Profiles.Create(resourceGroupName, profileName, new Profile(resourceLocation, new Sku(SkuName.StandardMicrosoft)));
            
        }

        private static void CreateCdnEndpoint(CdnManagementClient cdn)
        {          
                Console.WriteLine("Creating endpoint {0} on profile {1}.", endpointName, profileName);
                Endpoint endpointConfig =
                    new Endpoint()
                    {
                        //Origins = new List<DeepCreatedOrigin>() { new DeepCreatedOrigin("WiredbrainCoffee", "wbcoffee.azurewebsites.net") },

                        Origins = new List<DeepCreatedOrigin>() { new DeepCreatedOrigin("{Origin Server Name }", "{Origin Server Domain Name}") },
                        IsHttpAllowed = true,
                        IsHttpsAllowed = true,
                        Location = resourceLocation
                    };

                cdn.Endpoints.Create(resourceGroupName,profileName,endpointName,endpointConfig);
           
        }
    }
}

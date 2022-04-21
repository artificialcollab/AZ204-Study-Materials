using System;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace KeysDemo
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/key-vault/keys/quick-create-net?tabs=azure-cli
    /// </summary>
    class Program
    {
        const string _vaultName = "keyvault-psdemo01";
        const string _keyName = "key02";

        static void Main(string[] args)
        {
            Console.WriteLine("Working with Azure Key Vault Key API...");

            var kvUri = "https://" + _vaultName + ".vault.azure.net";

            var client = new KeyClient(new Uri(kvUri), new DefaultAzureCredential());

            var key = client.GetKeyAsync(_keyName).Result;

            Console.WriteLine($"[mydiskencryptionkey] value is {key.Value.KeyType}");

            Console.ReadLine();
        }
    }
}

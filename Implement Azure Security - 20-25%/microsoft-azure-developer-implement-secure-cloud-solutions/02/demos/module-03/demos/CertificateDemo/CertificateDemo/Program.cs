using System;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;

namespace CertificateDemo
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/key-vault/certificates/quick-create-net
    /// </summary>
    class Program
    {
        const string _vaultName = "keyvault-psdemo01";
        const string _certificateName = "mydemocertificate01";

        static void Main(string[] args)
        {
            Console.WriteLine("Working with Azure Key Vault Certificate API...");

            var kvUri = "https://" + _vaultName + ".vault.azure.net";

            var client = new CertificateClient(new Uri(kvUri), new DefaultAzureCredential());

            var certificate = client.GetCertificateAsync(_certificateName).Result;

            Console.WriteLine($"certificate: {certificate.Value.SecretId}");

            Console.ReadLine();
        }
    }
}

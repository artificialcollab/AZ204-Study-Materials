using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedisCache.Models;
using StackExchange.Redis;

namespace RedisCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var cacheConnectionString = _configuration.GetConnectionString("CacheConnection");

                return ConnectionMultiplexer.Connect(cacheConnectionString);
            });

            IDatabase cache = lazyConnection.Value.GetDatabase();

            var model = new CacheModel();

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Running the PING command");
            builder.AppendLine($"Response: {cache.Execute("PING").ToString()}");
            builder.AppendLine();

            builder.AppendLine("Running the FLUSHALL command");
            builder.AppendLine($"Response: {cache.Execute("FLUSHALL").ToString()}");
            builder.AppendLine();

            builder.AppendLine("GET the KEY: Message");
            builder.AppendLine($"Response: {cache.StringGet("Message").ToString()}");
            builder.AppendLine();

            builder.AppendLine("SET a KEY with value: Hello from ASP.NET");
            builder.AppendLine($"Response: {cache.StringSet("Message", "Hello from ASP.NET").ToString()}");
            builder.AppendLine();


            builder.AppendLine("GET the KEY: Message");
            builder.AppendLine($"Response: {cache.StringGet("Message").ToString()}");
            builder.AppendLine();

            builder.AppendLine("SET a KEY with value an Expiry");
            cache.StringSet("ExpiringMessage", "Hi, I expire", TimeSpan.FromSeconds(10));

            builder.AppendLine("GET the KEY: ExpiringMessage");
            builder.AppendLine($"ExpiringMessage: {cache.StringGet("Message").ToString()}");
            builder.AppendLine();


            builder.AppendLine("Wait for 30 seconds and then get the KEY ExpiryMessage");
            Thread.Sleep(10000);

            builder.AppendLine($"ExpiringMessage: {cache.StringGet("ExpiringMessage").ToString()}");
            builder.AppendLine();

            model.CacheLog = builder.ToString();

            // Get the client list

            model.CacheClients = cache.Execute("CLIENT", "LIST").ToString().Replace(
                                                                    "id=", "\rid=");
            lazyConnection.Value.Dispose();

            return View(model);
        }

      
    }
}

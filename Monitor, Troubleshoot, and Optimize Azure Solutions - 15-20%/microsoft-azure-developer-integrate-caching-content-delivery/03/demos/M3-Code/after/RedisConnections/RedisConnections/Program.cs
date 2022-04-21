using System;
using StackExchange.Redis;

namespace RedisConnections
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Redis retry example");
            SaveData();
            ReadData();

            Console.ReadKey();
            
        }

        public static void SaveData()
        {
            Console.WriteLine("Caching some data");
            var vehicleCount = 1000;
            var rnd = new Random();
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            

            for (int i = 1; i < vehicleCount; i++)
            {
                var speed = rnd.Next(0, 100);
                cache.StringSet($"Vehicle-{i}", speed,TimeSpan.FromMinutes(10));
            }
        }

        public static void ReadData()
        {
            Console.WriteLine("Reading some cached data");

            var cache = RedisConnectorHelper.Connection.GetDatabase();

            var vehicleCount = 1000;
            for (int i = 0; i < vehicleCount; i++)
            {
                var value = cache.StringGet($"Vehicle-{i}");
                Console.WriteLine($"Speed={value}");
            }
        }
    }

    public class RedisConnectorHelper
    {
        static RedisConnectorHelper()
        {
            var connectionString = "{REDIS CONNECTION STRING}";
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

            #region retryOptions

            // options.ReconnectRetryPolicy = new LinearRetry(5000);
            //retry#    retry to re-connect after time in milliseconds
            //1	        5000
            //2	        5000 	   
            //3	        5000 	   
            //4	        5000
            //5	        5000
            //6	        5000

            // options.ReconnectRetryPolicy = new ExponentialRetry(5000);
            // defaults maxDeltaBackoff to 10000 ms
            //retry#    retry to re-connect after time in milliseconds
            //1	        a random value between 5000 and 5500	   
            //2	        a random value between 5000 and 6050	   
            //3	        a random value between 5000 and 6655	   
            //4	        a random value between 5000 and 8053
            //5	        a random value between 5000 and 10000, since maxDeltaBackoff was 10000 ms
            //6	        a random value between 5000 and 10000
            #endregion

            options.ReconnectRetryPolicy = new ExponentialRetry(5000);
            //options.ReconnectRetryPolicy = new LinearRetry(5000);
            options.ConnectTimeout = 15000;

            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(options);
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}

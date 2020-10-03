using Confluent.Kafka;
using QueryBasedProducer.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryBasedProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // banner
                Console.WriteLine("Query Based Producer Started.");
                Console.WriteLine("Press CTRL+C to exit");

                var config = new ProducerConfig
                {
                    Acks = Acks.All,
                    BootstrapServers = "127.0.0.1:9092",
                    ClientId = Dns.GetHostName(),
                };

                var cancelled = false;
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                var tries = 0;
                var backoffTime = 0;

                while (!cancelled)
                {
                    // get name
                    Console.WriteLine("\nQuerying database for data...");

                    // query db
                    PurchaseOrder obj = null;

                    // exponential backoff (if there's no data slow the queries down.)
                    if (obj == null)
                    {
                        tries++;
                        var seconds = GetExponentialBackOff(tries);
                        backoffTime += seconds;
                        TimeSpan t = TimeSpan.FromSeconds(backoffTime);
                        string formatedBackoffTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s, t.Hours, t.Minutes, t.Seconds");
                        Console.WriteLine($"No data. Exponential backoff of {seconds} seconds. Backoff Time: {formatedBackoffTime}.");
                        await Task.Delay(seconds * 1000);
                        continue;
                    }

                    // reset tries & back off time
                    tries = 0; backoffTime = 0;

                    using (var producer = new ProducerBuilder<Null, string>(config).Build())
                    {
                        // convert the Msg model to json
                        var jsonMsg = JsonSerializer.Serialize(obj);

                        // push to Kafka
                        await producer.ProduceAsync("purchaseorders", new Message<Null, string> { Value = jsonMsg });
                    }

                    // produce message to Kafka
                    Console.WriteLine($"Purchase Order '{obj.PurchaseOrderId}' produced to Kafka.");
                    Console.WriteLine("\n");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static int GetExponentialBackOff(int tries)
        {
            return tries switch
            {
                1 => 1,
                2 => 5,
                3 => 10,
                _ => 15
            };
        }
    }
}

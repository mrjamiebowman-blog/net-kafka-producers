using Confluent.Kafka;
using QueryBasedProducer.Models;
using QueryBasedProducer.Repositories;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryBasedProducer
{
    class Program
    {
        private static IPurchaseOrderProducerRepository _purchaseOrderProdcerRepo;

        static async Task Main(string[] args)
        {
            try
            {
                // typically a dependency handler would inject this but for demo purposes ONLY...
                _purchaseOrderProdcerRepo = new SqlPurchaseOrderProducerRepository();

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
                    PurchaseOrder obj = await _purchaseOrderProdcerRepo.GetPurchaseOrderForProcessingAsync();

                    // exponential backoff (if there's no data slow the queries down.)
                    if (obj == null)
                    {
                        tries++;
                        var seconds = GetExponentialBackOff(tries);

                        // total backoff time
                        backoffTime += seconds;
                        TimeSpan t = TimeSpan.FromSeconds(backoffTime);
                        string formatedBackoffTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

                        // outout to console
                        Console.WriteLine($"No data. Exponential backoff of {seconds} seconds. Backoff Time: {formatedBackoffTime}.");

                        // backoff
                        await Task.Delay(seconds * 1000);
                        continue;
                    }

                    // reset tries & backoff time
                    tries = 0; backoffTime = 0;

                    using (var producer = new ProducerBuilder<Null, string>(config).Build())
                    {
                        // convert the Msg model to json
                        var jsonMsg = JsonSerializer.Serialize(obj);

                        // push to Kafka
                        await producer.ProduceAsync("purchaseorders", new Message<Null, string> { Value = jsonMsg });

                        // tombstone record
                        await _purchaseOrderProdcerRepo.TombstonePurchaseOrder(obj.PurchaseOrderId);
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

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Exercice1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> urls = readUrls();

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }

            // Create a listener.
            HttpListener listener = new HttpListener();

            // Trap Ctrl-C and exit 
            Console.CancelKeyPress += delegate
            {
                listener.Stop();
                System.Environment.Exit(0);
            };

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Received request for {request.Url}");
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";


                // Allow CORS
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");


                var json = JsonConvert.SerializeObject(getAges());
                var buffer = Encoding.UTF8.GetBytes(json);
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }

        }

        private static List<string> readUrls()
        {
            string filesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../urls.txt");
            string[] lines = File.ReadAllLines(filesPath);
            List<string> urls = lines.ToList();
            return urls;
        }

        private async static Task<AgeStats> getAges()
        {
            List<string> urls = readUrls();
            var httpClient = new HttpClient();
            Dictionary<string, int> ages = new Dictionary<string, int>();
            foreach (var url in urls)
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {

                    var ageHeader = response.Headers.FirstOrDefault(h => h.Key.Equals("Age", StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault();
                    if (!string.IsNullOrEmpty(ageHeader) && int.TryParse(ageHeader, out int age))
                    {
                        ages.Add(url, age);
                        Console.WriteLine($"\nAge of {url}: {age} seconds");
                    }
                }

                else
                {
                    Console.WriteLine($"Failed to get headers for {url}. Status code: {response.StatusCode}");
                }
            }




            foreach (KeyValuePair<string, int> pair in ages)
            {
                Console.WriteLine("\nPage {0} is aged {1} seconds.", pair.Key, pair.Value);
            }


            AgeStats agestats = new AgeStats();

            int res = 0;
            int count = 0;
            foreach (KeyValuePair<string, int> age in ages)
            {
                count++;
                res += age.Value;
            }

            agestats.AverageAge = res / count;
            agestats.StdDevAge = Math.Sqrt(ages.Values.Average(v => Math.Pow(v - agestats.AverageAge, 2)));

            Console.WriteLine("==========================================================");
            Console.WriteLine($"Average age {agestats.AverageAge}");
            Console.WriteLine($"Stanrdard Deviation of age {agestats.AverageAge}");
            Console.WriteLine("==========================================================");

            return agestats;

        }

        public class AgeStats
        {
            public int AverageAge { get; set; }
            public double StdDevAge { get; set; }
        }
    }


}


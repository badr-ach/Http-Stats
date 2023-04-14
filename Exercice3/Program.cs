using ConsoleApp3;
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


                var json = JsonConvert.SerializeObject(getStats());
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

        private async static Task<Stat> getStats()
        {
            List<string> urls = readUrls();
            var httpClient = new HttpClient();
            StatExtractor statExtractor = new StatExtractor();
            foreach (var url in urls)
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {

                    StatExtractor.extractStatsForHeader(statExtractor, response, url);
                }

                else
                {
                    Console.WriteLine($"Failed to get headers for {url}. Status code: {response.StatusCode}");
                }
            }

            Stat stat = new Stat();
            stat.cacheStatus = headerCount(statExtractor.cacheStatus);
            stat.setCookies = headerCount(statExtractor.setCookies);
            stat.varies = headerCount(statExtractor.varies);

            return stat;

        }

        private static Dictionary<string,int> headerCount(Dictionary<string,string> headerDict)
        {
            Dictionary<string, int> headerCount = new Dictionary<string, int>();

            foreach (KeyValuePair<string, string> keypair in headerDict)
            {
                if (headerCount.Keys.Contains(keypair.Value))
                {
                    headerCount[keypair.Value]++;
                }
                else
                {
                    headerCount.Add(keypair.Value, 1);
                }
            }
            return headerCount;
        }

        private static Dictionary<string, int> headerCount(Dictionary<string, List<string>> headerDict)
        {
            Dictionary<string, int> headerCount = new Dictionary<string, int>();

            foreach (KeyValuePair<string, List<string>> keypair in headerDict)
            {
                foreach(String value in keypair.Value)
                {
                    if (headerCount.Keys.Contains(value))
                    {
                        headerCount[value]++;
                    }
                    else
                    {
                        headerCount.Add(value, 1);
                    }
                }

            }
            return headerCount;
        }

        public class Stat
        {
            public Dictionary<string, int> setCookies { get; set; }
            public Dictionary<string, int> cacheStatus { get; set; }
            public Dictionary<string, int> varies { get; set; }

        }

    }


}


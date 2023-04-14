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


                var json = JsonConvert.SerializeObject(getServers());
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

        private async static Task<Dictionary<string, int>> getServers()
        {
            List<string> urls = readUrls();
            var httpClient = new HttpClient();
            Dictionary<string, string> servers = new Dictionary<string, string>();
            foreach (var url in urls)
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {

                    Console.WriteLine("=========================================");

                    Console.WriteLine($"Headers for {url}: \n");

                    foreach (var header in response.Headers.OrderBy(h => h.Key)) // trier les en-têtes dans l'ordre alphabétique
                    {
                        Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
                    }

                    Console.WriteLine("=========================================");
                    string server = response.Headers.FirstOrDefault(h => h.Key.Equals("Server")).Value?.FirstOrDefault();
                    if (!string.IsNullOrEmpty(server))
                    {
                        servers.Add(url, server);
                    }
                }

                else
                {
                    Console.WriteLine($"Failed to get headers for {url}. Status code: {response.StatusCode}");
                }
            }

            Dictionary<string, int> serverCounts = new Dictionary<string, int>();

            foreach (KeyValuePair<string, string> keypair in servers)
            {
                if (serverCounts.Keys.Contains(keypair.Value))
                {
                    serverCounts[keypair.Value]++;
                }
                else
                {
                    serverCounts.Add(keypair.Value, 1);
                }
            }


            Console.WriteLine("=========================================");
            
            foreach (KeyValuePair<string, int> pair in serverCounts)
            {
                Console.WriteLine("\n Server {0} is used {1} times.", pair.Key, pair.Value);
            }


            Console.WriteLine("=========================================");

            return serverCounts;
        }


    }
}

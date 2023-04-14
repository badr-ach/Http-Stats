using System.IO;
using System.Net;
using System.Text;
namespace Client
{
    using ConsoleApp3;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Hosting;

    class Program
    {

        private static void Main(string[] args)
        {


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

                // Allow CORS
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, Accept");


                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
                string fileContents = File.ReadAllText(filePath);

                // Construct a response.
                string responseString = fileContents;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }


        /*
        static async void Fetch(string[] args)
        {
            var httpClient = new HttpClient();
            var urls = new List<string>{
                "https://en.wikipedia.org/wiki/Artificial_intelligence",
                "https://en.wikipedia.org/wiki/Neural_network",
                "https://en.wikipedia.org/wiki/Computer_vision",
                "https://en.wikipedia.org/wiki/Machine_learning",
                "https://en.wikipedia.org/wiki/Data_mining",
                "https://en.wikipedia.org/wiki/Microsoft"
            };
            var ages = new List<int>();

            Stat stat = new Stat();            
            foreach (var url in urls)
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Headers for {url}:");
                    Stat.extractStatsForHeader(stat, response, url);

                    Console.WriteLine(); 
                }
                else
                {
                    Console.WriteLine($"Failed to get headers for {url}. Status code: {response.StatusCode}");
                }
            }
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($"Average age of pages: {stat.averageAge()} seconds");
                Console.WriteLine($"Standard deviation of age of pages: {stat.stdDevAge()} seconds");
                Console.WriteLine("===================================================");
                foreach (KeyValuePair<string, int> pair in stat.countByServer())
                {
                    Console.WriteLine("Server {0} is used {1} times.", pair.Key, pair.Value);
                }
                Console.WriteLine("---------------------------------------------------");

                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
        }*/
    }
}
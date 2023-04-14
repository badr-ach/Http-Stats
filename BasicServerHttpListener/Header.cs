using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class Header
    {
        private HttpWebResponse response;
        private WebHeaderCollection headers;

        public Header(HttpWebResponse response)
        {
            this.response = response;
            this.headers = response.Headers;
        }

        public void printStats()
        {
            Console.WriteLine("Server: " + response.Headers["Server"]);
            Console.WriteLine("Age: " + response.Headers["Age"]);
            Console.WriteLine("Strict-Transport-Security: " + response.Headers["Strict-Transport-Security"]);
            Console.WriteLine("X-XSS-Protection: " + response.Headers["X-XSS-Protection"]);
            Console.WriteLine("X-Content-Type-Options: " + response.Headers["X-Content-Type-Options"]);
            Console.WriteLine("Content-Security-Policy: " + response.Headers["Content-Security-Policy"]);
            Console.WriteLine("Set-Cookie: " + response.Headers["Set-Cookie"]);
            Console.WriteLine("Content-Encoding: " + response.Headers["Content-Encoding"]);
            Console.WriteLine("Content-Length: " + response.Headers["Content-Length"]);
            Console.WriteLine("Content-Type: " + response.Headers["Content-Type"]);
            Console.WriteLine("Date: " + response.Headers["Date"]);
            Console.WriteLine("Expires: " + response.Headers["Expires"]);
            Console.WriteLine("Last-Modified: " + response.Headers["Last-Modified"]);
        }

        public Fields getStats()
        {
            // Calcul des statistiques
            Fields stat = new Fields();

            // Serveur
            stat.server = response.Headers["Server"];
            stat.age = response.Headers["Age"];
            
            // Parcours de tous les en-têtes de la réponse
            foreach (string header in headers)
            {
                // En-têtes de sécurité
                if (header.ToLower().StartsWith("strict-transport-security"))
                {
                    stat.numStrictTransportSecurity++;
                    stat.numSecurityHeaders++;
                }

                if (header.ToLower().StartsWith("x-xss-protection"))
                {
                    stat.numXssProtection++;
                    stat.numSecurityHeaders++;
                }

                if (header.ToLower().StartsWith("x-content-type-options"))
                {
                    stat.numContentTypeOptions++;
                    stat.numSecurityHeaders++;
                }

                if (header.ToLower().StartsWith("content-security-policy"))
                {
                    stat.numContentSecurityPolicy++;
                    stat.numSecurityHeaders++;
                }

                // En-têtes de cookies
                if (header.ToLower().StartsWith("set-cookie"))
                {
                    stat.numCookieHeaders++;
                }

                // Codes de statut HTTP
                if (header.StartsWith("HTTP"))
                {
                    stat.numStatusCodes++;
                }

                // En-têtes de compression
                if (header.ToLower().StartsWith("content-encoding"))
                {
                    stat.numCompressionHeaders++;
                }

                // En-têtes de cache
                if (header.ToLower().StartsWith("cache-control"))
                {
                    stat.numCacheHeaders++;
                }

                // En-têtes de redirection
                if (header.ToLower().StartsWith("location"))
                {
                    stat.numRedirectHeaders++;
                }
            }
            return stat;
        }
        
        public class Fields
        {
            public string server { get; set; }
            public string age { get; set; }
            public int numSecurityHeaders { get; set; }
            public int numStrictTransportSecurity { get; set; }
            public int numXssProtection { get; set; }
            public int numContentTypeOptions { get; set; }
            public int numContentSecurityPolicy { get; set; }
            public int numCookieHeaders { get; set; }
            public int numStatusCodes { get; set; }
            public int numCompressionHeaders { get; set; }
            public int numCacheHeaders { get; set; }
            public int numRedirectHeaders { get; set; }
        }
    }

    
}

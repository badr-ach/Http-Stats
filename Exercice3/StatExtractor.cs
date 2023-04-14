using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ConsoleApp3
{
    internal class StatExtractor
    {
        public Dictionary<string, string> cacheStatus { get; set; }
        public Dictionary<string, List<string>> setCookies { get; set; }
        public Dictionary<string, List<string>> varies { get; set; }

        public StatExtractor()
        {
            cacheStatus = new Dictionary<string, string>();
            setCookies = new Dictionary<string, List<string>>();
            varies = new Dictionary<string, List<string>>();
        }

        public static void extractStatsForHeader(StatExtractor stat, HttpResponseMessage response, string url)
        {
            string cachestat = response.Headers.FirstOrDefault(h => h.Key.Equals("x-cache-status")).Value?.FirstOrDefault();
            if (!string.IsNullOrEmpty(cachestat))
            {
                Console.WriteLine($" {url} cache status is {cachestat}\n");
                stat.cacheStatus.Add(url, cachestat);
            }

            string setCookie = response.Headers.FirstOrDefault(h => h.Key.Equals("Set-Cookie")).Value?.FirstOrDefault();
            if (!string.IsNullOrEmpty(setCookie))
            {
                Console.WriteLine($" {url} want to set {setCookie}\n");
                string[] cookies = setCookie.Split(';');
                cookies.ToList().ForEach(c => {
                    if (!stat.setCookies.ContainsKey(url))
                    {
                        stat.setCookies.Add(url, new List<string>());
                    }
                    stat.setCookies[url].Add(c.Split('=')[0]);
                });
            }
            
            string vary = response.Headers.FirstOrDefault(h => h.Key.Equals("vary")).Value?.FirstOrDefault();
            if (!string.IsNullOrEmpty(vary))
            {
                Console.WriteLine($" {url} used these varies {vary}\n");
                string[] varies = vary.Split(',');
                varies.ToList().ForEach(c => {
                    if (!stat.varies.ContainsKey(url))
                    {
                        stat.varies.Add(url, new List<string>());
                    }
                    stat.varies[url].Add(c);
                });
            }
        }

        
    }
}

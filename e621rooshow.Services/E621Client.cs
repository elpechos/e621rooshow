using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace E621RooShow.Services
{
    public class E621Client
    {
        private const string BaseUrl = "https://e621.net/";


        /// <summary>
        /// Gets a page of posts, will return 100
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="beforeId"></param>
        /// <returns></returns>
        public PornPage GetPage(string tags, int limit = 75)
        {
            tags = tags.Trim();
            string url = BaseUrl + $"posts.json?tags=order:random {tags}";
            System.Diagnostics.Trace.WriteLine($"Search {url}");
            var xml = ReadTextFromUrl(url);
            var result = JsonConvert.DeserializeObject<PornPage>(xml);
            return result;
        }


        string ReadTextFromUrl(string url)
        {
            // WebClient is still convenient
            // Assume UTF8, but detect BOM - could also honor response charset I suppose
            using (var client = new WebClient())
            {
                //string userName = "cheeseypoofs";
                //string password = "vNSsN3FfmC37QU3pMHfxj3db";
                //string credentials = Convert.ToBase64String(
                //Encoding.ASCII.GetBytes(userName + ":" + password));
                //client.Headers.Add(HttpRequestHeader.Authorization,$"Basic {credentials}");
                client.Headers.Add("user-agent", "e621rooshow/1.0 (by cheeseypoofs on e621)");
                using (var stream = client.OpenRead(url))
                using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }
}

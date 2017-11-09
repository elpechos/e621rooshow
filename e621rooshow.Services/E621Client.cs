﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        public PornPage GetPage(string tags, int beforeId, int limit = 75)
        {
            tags = tags.Trim();
            string url = BaseUrl + $"post/index.xml?tags={tags}";
            if (beforeId != 0)
                url += $"&limit={limit}&before_id={beforeId + limit}";

            System.Diagnostics.Trace.WriteLine($"Search {url}");
            var xml = ReadTextFromUrl(url);
            var serializer = new XmlSerializer(typeof(PornPage));
            return (PornPage)serializer.Deserialize(new System.IO.StringReader(xml));
        }


        string ReadTextFromUrl(string url)
        {
            // WebClient is still convenient
            // Assume UTF8, but detect BOM - could also honor response charset I suppose
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "e621rooshow");
                using (var stream = client.OpenRead(url))
                using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }
}

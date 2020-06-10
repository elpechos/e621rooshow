using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace E621RooShow.Services
{
    public class PornPage
    {
        public Post[] Posts { get; set; }
    }

    public class Post
    {
        public File File { get; set; }

        public string Id { get; set; }

    }

    public class File
    {
        public string Md5 { get; set; }
        public string Ext { get; set; }
        public string Url { get; set; }


        public string RebuiltUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(Url))
                    return Url;

                if (Md5 != null)
                {
                    string baseUri = "https://static1.e621.net/data";
                    string path1 = string.Concat(Md5.Take(2));
                    string path2 = string.Concat(Md5.Skip(2).Take(2));
                    return $"{baseUri}/{path1}/{path2}/{Md5}.{Ext}";
                }

                return null;
            }
        }
    }
}

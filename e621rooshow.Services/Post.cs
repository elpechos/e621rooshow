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
        public string Url { get; set; }
    }
}

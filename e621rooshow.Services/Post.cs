using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace E621RooShow.Services
{
    [XmlRoot(ElementName = "posts")]
    public class PornPage
    {
        [XmlAttribute("count")]
        public long Count { get; set; }

        [XmlAttribute("offset")]
        public long Offset { get; set; }

        [XmlElement("post")]
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        [XmlElement("file_url")]
        public string file_url { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        public List<String> TagList
        {
            get
            {
                if (Tags == null)
                    return new List<string>();
                return Tags.ToLower().Split(' ').ToList();
            }
        }
    }
}

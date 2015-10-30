using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621RooShow.ViewModels
{
    public class FileDisplayInfo
    {
        public byte[] Data { get; set; }
        public string E621Url { get; set; }

    }


    public class FileDownloadInfo
    {
        public string DownloadUrl { get; set; }
        public string E621Url { get; set; }
    }
}

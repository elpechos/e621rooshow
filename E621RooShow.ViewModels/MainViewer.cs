using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Concurrent;
using E621RooShow.Services;
using E621RooShow.Services.Entropy;
using System.IO;
using System.Threading;
using E621RooShow.ViewModels.Properties;
using System.Drawing;

namespace E621RooShow.ViewModels
{
    public class MainViewer : INotifyPropertyChanged
    {

        //queue containing files to download
        private ConcurrentQueue<FileDownloadInfo> filesToDownload = new ConcurrentQueue<FileDownloadInfo>();
        //queue containing images to display
        private object filesToDisplayLock = new object();
        private List<FileDisplayInfo> filesToDisplay = new List<FileDisplayInfo>();
        private int imageIndex = 0;
        int maxPage = 10000;
        private List<string> allowedExtentions = new List<string>() { ".png", ".jpg" };

        public MainViewer()
        {
            WhiteList = Settings.Default.Tags;
            Interval = Settings.Default.Interval;
        }

        public void Start()
        {
            StartTask(DownloadFilesThread);
            StartTask(GetPagesToDownloadThread);
            StartTask(DisplayFileThread);
        }


        private void StartTask(Action task)
        {
            var thread = new Thread(new ThreadStart(task));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;
            thread.Start();
        }


        private void GetPagesToDownloadThread()
        {
            while (true)
            {
                if (filesToDownload.Count < 100)
                    try
                    {
                        GetPageToDownload();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                        System.Threading.Thread.Sleep(3000);
                    }
                else
                    System.Threading.Thread.Sleep(1000);
            }

        }

        private void GetPageToDownload()
        {
            E621Client client = new E621Client();

            int page = ThreadSafeRandom.ThisThreadsRandom.Next(maxPage);

            var dragonPorn = client.GetPage(WhiteList, page);

            maxPage = (int)(dragonPorn.Count / 75);
            var files = dragonPorn.Posts.ToList();
            files.Shuffle();


            foreach (var x in files)
            {
                var url = new Uri(x.file_url);
                var urlInfo = new FileInfo(url.AbsolutePath);

                if (allowedExtentions.Contains(urlInfo.Extension))
                    filesToDownload.Enqueue(new FileDownloadInfo() { DownloadUrl = x.file_url, E621Url = $"https://e621.net/post/show/{x.Id}/" });
            }
        }


        private void DownloadFilesThread()
        {
            while (true)
            {
                if (filesToDisplay.Count < 40)
                    try
                    {
                        DownloadFile();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex);
                        System.Threading.Thread.Sleep(3000);
                    }
                else
                    System.Threading.Thread.Sleep(1000);
            }
        }
        private void DownloadFile()
        {
            FileDownloadInfo fileToDownload = null;
            if (!filesToDownload.TryDequeue(out fileToDownload))
                return;
            using (var client = new System.Net.WebClient())
            {
                System.Diagnostics.Trace.WriteLine($"Downloading {fileToDownload.DownloadUrl} {filesToDownload.Count} remaining");
                var data = client.DownloadData(fileToDownload.DownloadUrl);
                lock (this.filesToDisplayLock)
                    filesToDisplay.Add(new FileDisplayInfo() { Data = data, E621Url = fileToDownload.E621Url });

            }
        }

        private void DisplayFileThread()
        {
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(Interval * 1000);
                    if (!isPaused)
                        DisplayFile();

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }

        private void DisplayFile()
        {
            lock (filesToDisplayLock)
            {
                if (this.imageIndex < 20)
                    this.imageIndex++;
                else
                    this.filesToDisplay.RemoveAt(0);

                if (this.imageIndex > 20)
                    this.imageIndex = 20;

                DisplayFile(this.imageIndex);
            }
        }

        private void DisplayFile(int index)
        {
            lock (filesToDisplayLock)
            {
                if (filesToDisplay.Count == 0)
                    return;

                if (index > (filesToDisplay.Count - 1))
                    index = filesToDisplay.Count - 1;

                if (index < 0)
                    return;

                this.CurrentImage = filesToDisplay[index];
                System.Diagnostics.Trace.WriteLine($"{filesToDisplay.Count} files left to display, index {index}");
            }
        }



        private void ClearData()
        {
            lock (this.filesToDisplayLock)
            {
                FileDownloadInfo ignored;
                while (filesToDownload.TryDequeue(out ignored)) ;
                filesToDisplay.Clear();
                this.imageIndex = 0;
            }
        }


        private void DisplayNext(int next)
        {
            lock (this.filesToDisplayLock)
            {
                this.imageIndex += next;

                if (this.imageIndex > this.filesToDisplay.Count - 1)
                    this.imageIndex = this.filesToDisplay.Count - 1;

                if (this.imageIndex < 0)
                    this.imageIndex = 0;

                DisplayFile(this.imageIndex);
            }
        }


        private FileDisplayInfo _currentImage;
        public FileDisplayInfo CurrentImage
        {

            get
            {
                return _currentImage;
            }

            private set
            {
                if (_currentImage != value)
                {
                    _currentImage = value;
                    OnPropertyChanged("CurrentImage");
                }
            }
        }
        public string WhiteList
        {

            get
            {
                return Settings.Default.Tags;
            }

            set
            {
                Settings.Default.Tags = value;
                Settings.Default.Save();
                ClearData();
            }
        }


        private int _interval = 5;
        public int Interval
        {
            get
            {
                return Settings.Default.Interval;
            }
            set
            {
                if (_interval <= 0)
                    throw new ArgumentException("Interval must be greater than 0");
                Settings.Default.Interval = value;
                Settings.Default.Save();
            }
        }

        private bool isPaused = false;
        public void Pause()
        {
            isPaused = !isPaused;
        }

        public void Next()
        {
            DisplayNext(1);
        }

        public void Back()
        {
            DisplayNext(-1);
        }

        public string Title => "Matthew Roo's E621 Slideshow!";


        public Color BackgroundColor
        {
            get
            {
                return Settings.Default.BackgroundColor;
            }
            set
            {
                Settings.Default.BackgroundColor = value;
                Settings.Default.Save();
            }
        }

        public Color StatusColor
        {
            get
            {
                return Settings.Default.StatusColor;
            }
            set
            {
                Settings.Default.StatusColor = value;
                Settings.Default.Save();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using e621rooshow.Properties;
using e621rooshow.XmlApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace e621rooshow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //queue containing files to download
        private ConcurrentQueue<string> filesToDownload = new ConcurrentQueue<string>();

        //queue containing images to display
        private ConcurrentQueue<byte[]> filesToDisplay = new ConcurrentQueue<byte[]>();



        System.Windows.Threading.DispatcherTimer displayTimer;

        public MainWindow()
        {
            InitializeComponent();
            displayTimer = new DispatcherTimer(TimeSpan.FromSeconds(Settings.Default.Interval), DispatcherPriority.Background, (s, e) => DisplayFile(), this.Dispatcher);

            var thread = new Thread(new ThreadStart(DownloadFiles));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;
            thread.Start();

            thread = new Thread(new ThreadStart(GetFilesToDownload));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;
            thread.Start();

            blackList = (Settings.Default.TagsBlacklist ?? string.Empty).ToLower().Split(' ').ToList();
        }


        private void DisplayFile()
        {
            byte[] fileToDisplay = null;
            if (!filesToDisplay.TryDequeue(out fileToDisplay))
                return;
            Image.Source = LoadImage(fileToDisplay);

            System.Diagnostics.Trace.WriteLine($"{filesToDisplay.Count} files left to display");
        }


        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }


        private void DownloadFiles()
        {
            while (true)
            {
                if (filesToDisplay.Count < 19)
                    try
                    {
                        DownloadFile();
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Thread.Sleep(3000);
                        System.Diagnostics.Trace.WriteLine(ex);
                    }
                else
                    System.Threading.Thread.Sleep(1000);
            }
        }
        private void DownloadFile()
        {
            string fileToDownload = null;
            if (!filesToDownload.TryDequeue(out fileToDownload))
                return;
            using (var client = new System.Net.WebClient())
            {
                System.Diagnostics.Trace.WriteLine($"Downloading {fileToDownload} {filesToDownload.Count} remaining");
                filesToDisplay.Enqueue(client.DownloadData(fileToDownload));
            }
        }

        int maxPage = 10000;
        List<string> blackList = new List<string>();
        private List<string> allowedExtentions = new List<string>() { ".png", ".jpg" };

        private void GetFilesToDownload()
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
            //Test, get 2nd page of dragon porn
            var dragonPorn = client.GetPage(Settings.Default.Tags.ToLower(), page);
            maxPage = (int)(dragonPorn.Count / 75);
            var files = dragonPorn.Posts.ToList();
            files.Shuffle();


            foreach (var x in files)
            {
                if (blackList.Intersect(x.TagList).Any())
                    continue;

                var url = new Uri(x.file_url);
                var urlInfo = new FileInfo(url.AbsolutePath);

                if (allowedExtentions.Contains(urlInfo.Extension))
                    filesToDownload.Enqueue(x.file_url);
            }
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }


        private void UpdateInterval(int interval)
        {
            Settings.Default.Interval = interval;
            displayTimer.Interval = TimeSpan.FromSeconds(Settings.Default.Interval);
            Settings.Default.Save();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => UpdateInterval(1);
        private void MenuItem_Click_2(object sender, RoutedEventArgs e) => UpdateInterval(2);
        private void MenuItem_Click_3(object sender, RoutedEventArgs e) => UpdateInterval(5);
        private void MenuItem_Click_4(object sender, RoutedEventArgs e) => UpdateInterval(10);
        private void MenuItem_Click_5(object sender, RoutedEventArgs e) => UpdateInterval(20);

        private void MenuItem_Click_6(object sender, RoutedEventArgs e) => UpdateInterval(30);

        private void MenuItem_Click_7(object sender, RoutedEventArgs e) => UpdateInterval(60);

        private void MenuItem_Click_8(object sender, RoutedEventArgs e) => UpdateInterval(120);

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            Settings.Default.Tags = new InputBox("Enter list of tags seperated by spaces", "Tags", Settings.Default.Tags.ToLower()).ShowDialog().ToLower();
            Settings.Default.Save();
            ClearData();
        }

        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            var blackListString = (new InputBox("Enter list of tags to blacklist seperated by spaces", "Tags", string.Join(" ", blackList)).ShowDialog() ?? string.Empty)
                            .ToLower();

            Settings.Default.TagsBlacklist = blackListString;

            blackList = Settings.Default.TagsBlacklist.Split(' ').ToList();

            Settings.Default.Save();

            ClearData();
        }

        private void ClearData()
        {
            string ignored;
            while (filesToDownload.TryDequeue(out ignored)) ;

            byte[] crap;

            while (filesToDisplay.TryDequeue(out crap)) ;
        }
    }
}

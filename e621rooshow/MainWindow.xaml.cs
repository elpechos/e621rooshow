using E621RooShow.ViewModels;
using E621RooShow.Windows.Properties;
using E621RooShow.XmlApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
namespace E621RooShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        protected MainViewer MainViewer = new MainViewer();

        public MainWindow()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(ToMediaColor(Settings.Default.BackgroundColor));
            MainViewer.BlackList = Settings.Default.TagsBlacklist;
            MainViewer.WhiteList = Settings.Default.Tags;
            MainViewer.Interval = Settings.Default.Interval;
            MainViewer.PropertyChanged += MainViewer_PropertyChanged;
            MainViewer.Start();
        }

        private void MainViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Image.Dispatcher.Invoke(
                () =>
                    {
                        Trace.WriteLine("Image Changed");
                        Image.Source = LoadImage(MainViewer.CurrentImage.Data);
                        Image.Reset();
                    }
                );
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


        private void UpdateInterval(int interval)
        {
            Settings.Default.Interval = interval;
            MainViewer.Interval = Settings.Default.Interval;
            Settings.Default.Save();
        }
        private void MenuItem_Click_Tags(object sender, RoutedEventArgs e)
        {
            Settings.Default.Tags = new InputBox("Enter list of tags seperated by spaces", "Tags", Settings.Default.Tags.ToLower()).ShowDialog().ToLower();
            MainViewer.WhiteList = Settings.Default.Tags;
            Settings.Default.Save();
        }

        private void MenuItem_Click_Blacklist(object sender, RoutedEventArgs e)
        {
            var blackListString = (new InputBox("Enter list of tags to blacklist seperated by spaces", "Tags", string.Join(" ", MainViewer.BlackList)).ShowDialog() ?? string.Empty)
                            .ToLower();
            Settings.Default.TagsBlacklist = blackListString;
            MainViewer.BlackList = Settings.Default.TagsBlacklist;
            Settings.Default.Save();
        }


        private bool isFullScreen = false;
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Space:
                    MainViewer.Pause();
                    break;

                case Key.Left:
                    MainViewer.Back();
                    break;

                case Key.Right:
                    MainViewer.Next();
                    break;

                case Key.F:
                    FullScreen = !FullScreen;
                    break;

                case Key.Enter:
                    Process.Start(MainViewer.CurrentImage.E621Url);
                    break;
            }
        }

        private bool _isFullScreen = false;
        protected bool FullScreen
        {
            get
            {
                return _isFullScreen;
            }
            set
            {
                if (value)
                {
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                    Hide();
                    Show();
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                _isFullScreen = value;
            }
        }


        private void MenuItem_Click_1_Second(object sender, RoutedEventArgs e) => UpdateInterval(1);
        private void MenuItem_Click_2_Second(object sender, RoutedEventArgs e) => UpdateInterval(2);
        private void MenuItem_Click_5_Second(object sender, RoutedEventArgs e) => UpdateInterval(5);
        private void MenuItem_Click_10_Second(object sender, RoutedEventArgs e) => UpdateInterval(10);
        private void MenuItem_Click_20_Second(object sender, RoutedEventArgs e) => UpdateInterval(20);
        private void MenuItem_Click_30_Second(object sender, RoutedEventArgs e) => UpdateInterval(30);
        private void MenuItem_Click_60_Second(object sender, RoutedEventArgs e) => UpdateInterval(60);
        private void MenuItem_Click_120_Second(object sender, RoutedEventArgs e) => UpdateInterval(120);


        public static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}

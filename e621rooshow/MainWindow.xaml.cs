using E621RooShow.ViewModels;
using E621RooShow.Windows.Properties;
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
using E621RooShow.Windows.ScreenManagement;
using System.ComponentModel;
using E621RooShow.ViewModels.TraceListeners;

namespace E621RooShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        protected MainViewer MainViewer = new MainViewer();

        public MainWindow()
        {
            this.DataContext = this;
            Listener = new LabelTraceWriter();
            System.Diagnostics.Trace.Listeners.Add(Listener);
            InitializeComponent();
            this.Background = new SolidColorBrush(ToMediaColor(MainViewer.BackgroundColor));
            //this.traceLabel.Foreground = new SolidColorBrush(ToMediaColor(MainViewer.StatusColor));
            MainViewer.PropertyChanged += MainViewer_PropertyChanged;
            MainViewer.Start();
        }

        public LabelTraceWriter Listener { get; }

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
            MainViewer.Interval = interval;
        }
        private void MenuItem_Click_Tags(object sender, RoutedEventArgs e)
        {
            MainViewer.WhiteList = new InputBox("Enter list of tags seperated by spaces, prefix with - to blacklist (dragon -fox)", "Tags", MainViewer.WhiteList).ShowDialog().ToLower();
        }

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
                    //SleepUtil.PreventSleep();
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
                    this.Topmost = true;
                    Hide();
                    Show();

                }
                else
                {
                    this.Topmost = false;
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                _isFullScreen = value;
            }
        }

        public Visibility ShowStatus { get; set; }

        private void MenuItem_Click_1_Second(object sender, RoutedEventArgs e) => UpdateInterval(1);
        private void MenuItem_Click_2_Second(object sender, RoutedEventArgs e) => UpdateInterval(2);
        private void MenuItem_Click_5_Second(object sender, RoutedEventArgs e) => UpdateInterval(5);
        private void MenuItem_Click_10_Second(object sender, RoutedEventArgs e) => UpdateInterval(10);
        private void MenuItem_Click_20_Second(object sender, RoutedEventArgs e) => UpdateInterval(20);
        private void MenuItem_Click_30_Second(object sender, RoutedEventArgs e) => UpdateInterval(30);
        private void MenuItem_Click_60_Second(object sender, RoutedEventArgs e) => UpdateInterval(60);
        private void MenuItem_Click_120_Second(object sender, RoutedEventArgs e) => UpdateInterval(120);

        private void MenuItem_Status(object sender, RoutedEventArgs e)
        {
            var oldValue = this.ShowStatus;
            if (this.ShowStatus == Visibility.Visible)
                this.ShowStatus = Visibility.Hidden;
            else
                this.ShowStatus = Visibility.Visible;

            OnPropertyChanged("ShowStatus");
        }


        public static MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FullScreen = !FullScreen;
        }
    }
}

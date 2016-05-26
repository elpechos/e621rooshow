using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using E621RooShow.ViewModels;
using System.IO;
using E621RooShow.Linux.Controls;

namespace E621RooShow.Linux
{
    public partial class MainWindow : Window
    {
        protected MainViewer MainViewer;


        public MainWindow(MainViewer viewModel) : base(viewModel.Title)
        {
            this.MainViewer = viewModel;

            var bg = MainViewer.BackgroundColor;
            this.ModifyBg(StateType.Normal, new Gdk.Color(bg.R, bg.G, bg.B));
            AddEvents((int)Gdk.EventMask.ButtonPressMask | (int)Gdk.EventMask.ScrollMask | (int)Gdk.EventMask.KeyPressMask);
            MainViewer.PropertyChanged += MainViewer_PropertyChanged;
            CreateControls();



            this.MainViewer.Start();
        }


        private void MenuItem_Click_Tags(object o, ButtonPressEventArgs args)
        {
            MainViewer.WhiteList = Popup("Enter list of tags seperated by spaces", "Tags", MainViewer.WhiteList).ToLower();
        }

        private object currentPixbufLock = new object();
        Gdk.Pixbuf currentPixbuf;
        private void MainViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lock (currentPixbufLock)
            using (var stream = new MemoryStream(this.MainViewer.CurrentImage.Data))
                {
                    if (currentPixbuf != null)
                        currentPixbuf.Dispose();

                    currentPixbuf = new Gdk.Pixbuf(stream);
                    QueueResize();
                }
        }

        protected override bool OnScrollEvent(Gdk.EventScroll evnt)
        {
            return base.OnScrollEvent(evnt);
        }
        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            if (evnt.Button == 3) /* right click */
            {
                ContextMenu.ShowAll();
                ContextMenu.Popup();
            }

            return true;
        }

        protected override bool OnKeyPressEvent(Gdk.EventKey evnt)
        {
            switch (evnt.Key)
            {
                case Gdk.Key.Left:
                    MainViewer.Back();
                    break;

                case Gdk.Key.Right:
                    MainViewer.Next();
                    break;

                case Gdk.Key.space:
                    MainViewer.Pause();
                    break;

                case Gdk.Key.Return:
                    //MainViewer.Exec();
                    break;
            }

            return base.OnKeyPressEvent(evnt);
        }


        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            lock (currentPixbufLock)
            {
                if (currentPixbuf == null)
                    return true;

                int width, height;
                GetSize(out width, out height);

                var maxSize = Tuple.Create(width, height);
                var originalSize = Tuple.Create(currentPixbuf.Width, currentPixbuf.Height);
                var newSize = ImageAspectRatio.ResizeFit(originalSize, maxSize);
                var newPosition = ImageAspectRatio.Center(newSize, maxSize);

                using (var newPixBuf = currentPixbuf.ScaleSimple(newSize.Item1, newSize.Item2, Gdk.InterpType.Hyper))
                using (var gc = new Gdk.GC(evnt.Window))
                {
                    evnt.Window.DrawPixbuf(gc, newPixBuf, 0, 0, newPosition.Item1, newPosition.Item2, newSize.Item1, newSize.Item2, Gdk.RgbDither.None, 0, 0);
                }
            }
            return true;
        }

        private void MenuItem_Click_1_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(1);
        private void MenuItem_Click_2_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(2);
        private void MenuItem_Click_5_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(5);
        private void MenuItem_Click_10_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(10);
        private void MenuItem_Click_20_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(20);
        private void MenuItem_Click_30_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(30);
        private void MenuItem_Click_60_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(60);
        private void MenuItem_Click_120_Second(object sender, ButtonPressEventArgs e) => UpdateInterval(120);

        private void UpdateInterval(int interval)
        {
            MainViewer.Interval = interval;
        }


        public string Popup(string content, string title, string defaultText)
        {
            var box = new InputBox(title, content, defaultText);
            box.Parent = this;
            box.DestroyWithParent = true;
            box.Modal = true;
            box.Run();
            return box.Text;
        }
    }
}

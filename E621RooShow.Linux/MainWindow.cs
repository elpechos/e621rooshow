using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using E621RooShow.ViewModels;
using System.IO;


namespace E621RooShow.Linux
{
    public partial class MainWindow : Window
    {
        protected MainViewer MainViewer;


        public MainWindow(MainViewer viewModel) : base(viewModel.Title)
        {
            AddEvents((int)Gdk.EventMask.ButtonPressMask);
            this.MainViewer = viewModel;
            MainViewer.PropertyChanged += MainViewer_PropertyChanged;
            CreateControls();

            this.MainViewer.Start();
        }


        private void Tags_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {

        }

        private void Blacklist_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            
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

        protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
        {
            if (evnt.Button == 3) /* right click */
            {
                ContextMenu.ShowAll();
                ContextMenu.Popup();
            }

            return true;
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

                using (var newPixBuf = currentPixbuf.ScaleSimple(newSize.Item1, newSize.Item2, Gdk.InterpType.Nearest))
                using (var gc = new Gdk.GC(evnt.Window))
                {
                    evnt.Window.DrawPixbuf(gc, newPixBuf, 0, 0, newPosition.Item1, newPosition.Item2, newSize.Item1, newSize.Item2, Gdk.RgbDither.None, 0, 0);
                }
            }
            return true;
        }
    }
}

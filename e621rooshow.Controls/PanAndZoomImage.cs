using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace e621rooshow.Controls
{
    public class PanAndZoomImage : Image
    {
        private UIElement border;
        public PanAndZoomImage()
        {


            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            RenderTransform = group;
            RenderTransformOrigin = new Point(0.0, 0.0);

            this.MouseWheel += image_MouseWheel;
            this.MouseLeftButtonDown += image_MouseLeftButtonDown;
            this.MouseLeftButtonUp += image_MouseLeftButtonUp;
            this.MouseMove += child_MouseMove;
            border = VisualTreeHelper.GetParent(this) as UIElement;
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = GetScaleTransform();
            double zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }

        Point start;
        Point origin;
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tt = GetTranslateTransform();
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
            this.Cursor = Cursors.Hand;
            CaptureMouse();
        }


        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }


        private TranslateTransform GetTranslateTransform()
        {
            var element = this;
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform()
        {
            var element = this;
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var st = GetScaleTransform();
                var tt = GetTranslateTransform();
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - (v.X);
                tt.Y = origin.Y - (v.Y);
            }
        }


        public void Reset()
        {
            // reset zoom
            var st = GetScaleTransform();
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform();
            tt.X = 0.0;
            tt.Y = 0.0;
        }

    }
}

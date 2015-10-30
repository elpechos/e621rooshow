using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace E621RooShow
{
    public class InputBox : Window
    {
        StackPanel sp1 = new StackPanel();// items container
        string title = "InputBox";//title as heading
        string boxcontent;//title
        string defaulttext = "";//default textbox content
        string okbuttontext = "OK";//Ok button content
        TextBox input = new TextBox();
        Button ok = new Button();
        public InputBox(string content)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            windowdef();
        }
        public InputBox(string content, string Htitle, string DefaultText)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            try
            {
                defaulttext = DefaultText;
            }
            catch
            {
                DefaultText = "Error!";
            }
            windowdef();
        }
        public InputBox(string content, string Htitle, string Font, int Fontsize)
        {

            boxcontent = content;
            title = Htitle;
            if (Fontsize >= 1)
                FontSize = Fontsize;
            windowdef();
        }

        protected override void OnActivated(EventArgs e)
        {
            input.Focus();
            base.OnActivated(e);
        }

        private void windowdef()// window building - check only for window size
        {
            Height = 120;// Box Height
            Width = 500;// Box Width
            Title = title;
            Content = sp1;
            Closing += Box_Closing;
            TextBlock content = new TextBlock();
            content.TextWrapping = TextWrapping.Wrap;
            content.Background = null;
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.Text = boxcontent;
            content.FontSize = FontSize;
            sp1.Children.Add(content);
            input.FontSize = FontSize;
            input.Text = defaulttext;
            input.Margin = new Thickness(5);
            input.KeyUp += Input_KeyUp;
            input.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp1.Children.Add(input);
            ok.Width = 70;
            ok.Height = 30;
            ok.Click += ok_Click;
            ok.Content = okbuttontext;
            ok.HorizontalAlignment = HorizontalAlignment.Center;
            sp1.Children.Add(ok);
        }

        private void Input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Close();
        }

        void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        void ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public new string ShowDialog()
        {
            base.ShowDialog();
            return input.Text;
        }


    }
}

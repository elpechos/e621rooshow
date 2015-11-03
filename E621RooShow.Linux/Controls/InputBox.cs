using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621RooShow.Linux.Controls
{
    public class InputBox : Dialog
    {


        Entry entry;
        public InputBox(string title, string prompt, string defaultText) : base()

        {
            this.AddEvents((int)Gdk.EventMask.KeyPressMask);
            this.KeyPressEvent += Entry_KeyPressEvent;

            this.Title = title;
            var label = new Label(prompt);
            this.VBox.Add(label);
            entry = new Entry();
            entry.Text = defaultText;
            VBox.Add(entry);
            AddButton(Stock.Ok, ResponseType.Ok);
            VBox.ShowAll();
        }

        [GLib.ConnectBefore()]
        private void Entry_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Return)
                OnResponse(ResponseType.Ok);
        }

        private string _text;
        protected override void OnResponse(ResponseType response_id)
        {
            _text = entry.Text;
            this.Destroy();
        }
        public string Text
        {
            get
            {
                return _text;
            }
        }
    }
}

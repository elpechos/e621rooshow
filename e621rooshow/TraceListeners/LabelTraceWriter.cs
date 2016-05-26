﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace E621RooShow.Windows.TraceWriters
{
    public class LabelTraceWriter : System.Diagnostics.TraceListener, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Text { get; set; }


        public override void Write(string message)
        {
            this.Text = message;
            OnPropertyChanged("Text");
        }

        public override void WriteLine(string message)
        {
            this.Text = message;
            OnPropertyChanged("Text");
        }
    }
}

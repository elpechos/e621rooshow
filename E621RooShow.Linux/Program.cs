﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
namespace E621RooShow.Linux
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();

            //Create the Window
            Window myWin = new MainWindow(new ViewModels.MainViewer());
            myWin.Resize(200, 200);

            //Show Everything     
            myWin.ShowAll();

            Application.Run();
        }
    }
}

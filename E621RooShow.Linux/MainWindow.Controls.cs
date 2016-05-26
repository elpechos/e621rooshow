using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
namespace E621RooShow.Linux
{
    public partial class MainWindow
    {

        Menu ContextMenu = new Menu();
        
        
        private Image CurrentImage;
        protected void CreateControls()
        {
            CurrentImage = new Image();
            Add(CurrentImage);
            AddItemToMenu("Tags,", MenuItem_Click_Tags);
            AddItemToMenu("1 second", MenuItem_Click_1_Second);
            AddItemToMenu("2 seconds", MenuItem_Click_2_Second);
            AddItemToMenu("5 seconds", MenuItem_Click_5_Second);
            AddItemToMenu("10 seconds", MenuItem_Click_10_Second);
            AddItemToMenu("20 seconds", MenuItem_Click_20_Second);
            AddItemToMenu("30 seconds", MenuItem_Click_30_Second);
            AddItemToMenu("60 seconds", MenuItem_Click_60_Second);
            AddItemToMenu("120 seconds", MenuItem_Click_120_Second);
        }



        private void AddItemToMenu(string text, ButtonPressEventHandler buttonPress)
        {
            var menuItem = new MenuItem(text);
            menuItem.ButtonPressEvent += buttonPress;
            ContextMenu.Add(menuItem);
        }
    }
}

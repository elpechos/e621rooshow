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

            ContextMenu.Add(new MenuItem("Tags") { Name = "tags" });
            ContextMenu.Add(new MenuItem("Tags Black-list") { Name = "blacklist" });
            ContextMenu.Add(new MenuItem("1 second"));
        }
    }
}

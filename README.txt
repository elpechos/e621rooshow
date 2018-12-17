# e621rooshow
An application to display a slideshow from e621

## Controls
* Right click - Set display interval, tags, and tags-blacklist
* Space - Pause
* F - Fullscreen
* Left and right arrow keys - Browse through buffered images
* Enter - Brings up the e621 post web-page for the current image.
* Mousewheel - Zoom
* Left click and drag - Pans the image



## Windows Install Instructions

Unzip to a folder
Double click "E621RooShow.Windows.exe"

## Linux Install Instructions

The linux version of the application requires the mono runtime as well as GTK bindings.
This is what works for me on Ubuntu:

apt-get install mono-runtime
apt-get install libgtk2.0-cil
apt-get install libmono-system-core4.0-cil
mono E621RooShow.Linux.exe

If people could send me instructions for other distributions I'd be happy to include them.

## Configuration

In the .config file you can change the background colour by setting a RGB value from 0,0,0 to 255,255,255

Enjoy!


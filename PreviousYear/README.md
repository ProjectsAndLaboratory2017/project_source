# GadgeteerPLCS
Project developed during the "Project And Laboratory on Communication Systems" course
in collaboartion with Eugenio Cardonatti and Tommaso Guario

HelloLED <-- gadgeteer code

WpfApplication1_final <-- server code

sito <-- Website made to show MySql data saved by server

The board take a foto, send the foto to the server, then if a face is detect (server use EMGU CV) the board unlocks.
Image data is stored in a MySQL database on Server. 
After some wrong attemp to unlock the board with an image a pin pad is shown to unlock by code.

Due to problems with the network interface of FezSpiderII (board loses packets during transmission)
the image is splitted in several small packets before it is sent to the server. 
That helped in reducing trasmission time since board can recover quickly from losses of small packets.


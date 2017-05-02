using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

using GHI.Networking;
//using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;

using System.Net;

namespace HttpClient
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            
            Debug.Print("Program Started");

            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            ethernetJ11D.NetworkInterface.Open();
            //ethernetJ11D.NetworkInterface.EnableDhcp();
            ethernetJ11D.NetworkInterface.EnableStaticIP("192.168.1.2", "255.255.255.0", "192.168.1.1");
            string[] dns = { "192.168.1.1" };
            ethernetJ11D.NetworkInterface.EnableStaticDns(dns);
            //ethernetJ11D.NetworkInterface.EnableDynamicDns();

            while (ethernetJ11D.NetworkInterface.IPAddress == "0.0.0.0")
            {
                Debug.Print("Waiting for DHCP");
                Thread.Sleep(250);
            }

            //The network is now ready to use.

            while (ethernetJ11D.NetworkInterface.IPAddress == "0.0.0.0")
            {
                Debug.Print("Waiting for DHCP");
                Thread.Sleep(250);
            }
            button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
            //The network is now ready to use.
        }

        private void NetworkChange_NetworkAddressChanged(object sender, Microsoft.SPOT.EventArgs e)
        {
            Debug.Print("Network address changed: " + ethernetJ11D.NetworkInterface.IPAddress);
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Debug.Print("Network availability: " + e.IsAvailable.ToString());
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            byte[] result = new byte[65536];
            int read = 0;

            using (var req = HttpWebRequest.Create("http://192.168.1.1/") as HttpWebRequest)
            {
                using (var res = req.GetResponse() as HttpWebResponse)
                {
                    using (var stream = res.GetResponseStream())
                    {
                        do
                        {
                            read = stream.Read(result, 0, result.Length);
                            Debug.Print("received" + result.Length);
                            Thread.Sleep(20);
                        } while (read != 0);
                    }
                }
            }
        }
    }
}

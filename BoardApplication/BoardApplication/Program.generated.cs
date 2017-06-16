//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BoardApplication {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Camera module using socket 3 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Camera camera;
        
        /// <summary>The Button module using socket 4 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Button button;
        
        /// <summary>The USB Client EDP module using socket 1 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.USBClientEDP usbClientEDP;
        
        /// <summary>The Display TE35 module using sockets 14, 13, 12 and 10 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.DisplayTE35 displayTE35;
        
        /// <summary>The Ethernet J11D module using socket 7 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.EthernetJ11D ethernetJ11D;
        
        /// <summary>The Tunes module using socket 8 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Tunes tunes;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZSpiderII Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZSpiderII)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZSpiderII();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }

       
        
        private void InitializeModules() {
            this.camera = new GTM.GHIElectronics.Camera(3);
            this.button = new GTM.GHIElectronics.Button(4);
            this.usbClientEDP = new GTM.GHIElectronics.USBClientEDP(1);
            this.displayTE35 = new GTM.GHIElectronics.DisplayTE35(14, 13, 12, 10);
            this.ethernetJ11D = new GTM.GHIElectronics.EthernetJ11D(7);
            this.tunes = new GTM.GHIElectronics.Tunes(8);
        }
    }
}

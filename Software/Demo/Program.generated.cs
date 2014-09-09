//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kit_Demo {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Tunes module using socket 3 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Tunes tunes;
        
        /// <summary>The UsbClientSP module using socket 8 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.UsbClientSP usbClientSP;
        
        /// <summary>The Joystick module using socket 4 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Joystick joystick;
        
        /// <summary>The Display N18 module using socket 6 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Display_N18 display_N18;
        
        /// <summary>The LED Strip module using socket 7 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.LED_Strip led_Strip;
        
        /// <summary>The Button module using socket 5 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Button button;
        
        /// <summary>The LedLevel module using socket 2 of the mainboard.</summary>
        private Gadgeteer.Modules.IngenuityMicro.LedLevel ledLevel;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZCerberus Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZCerberus)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZCerberus();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.tunes = new GTM.GHIElectronics.Tunes(3);
            this.usbClientSP = new GTM.GHIElectronics.UsbClientSP(8);
            this.joystick = new GTM.GHIElectronics.Joystick(4);
            this.display_N18 = new GTM.GHIElectronics.Display_N18(6);
            this.led_Strip = new GTM.GHIElectronics.LED_Strip(7);
            this.button = new GTM.GHIElectronics.Button(5);
            this.ledLevel = new GTM.IngenuityMicro.LedLevel(2);
        }
    }
}

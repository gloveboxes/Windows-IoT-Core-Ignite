using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Glovebox.Graphics.Components;
using Glovebox.Graphics.Drivers;
using Glovebox.IoT.Devices.Converters;
using Glovebox.IoT.Devices.Sensors;
using System.Diagnostics;
using System.Threading.Tasks;
using AdapterLib;
using BridgeRT;
using Windows.Foundation;


// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RPiDemo
{
    public sealed class StartupTask : IBackgroundTask
    {

        BackgroundTaskDeferral _deferral;

        LED8x8Matrix matrix = new LED8x8Matrix(new Ht16K33());
        LED8x8Matrix strip = new LED8x8Matrix(new MAX7219(4, MAX7219.Rotate.None, MAX7219.Transform.HorizontalFlip));

        BMP180 bmp180 = new BMP180();

        AdcProviderManager adcManager = new AdcProviderManager();

        Adapter adapter = new Adapter();
        private DsbBridge dsbBridge;


        Ldr light;


        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            _deferral = taskInstance.GetDeferral();

            dsbBridge = new DsbBridge(adapter);

            adapter.AllJoynSet += Adapter_AllJoynSet;

            var initResult = dsbBridge.Initialize();
            if (initResult != 0)
            {
                throw new Exception("DSB Bridge initialization failed!");
            }



            adcManager.Providers.Add(new ADS1015(ADS1015.Gain.Volt33));
            var ads1015 = (await adcManager.GetControllersAsync())[0];
            light = new Ldr(ads1015.OpenChannel(2));
            

            matrix.SetBrightness(1);
            strip.SetBrightness(1);

            ShowTempPressure();
            ShowLight();


        }

        async void ShowTempPressure()
        {
            while (true)
            {
                string msg = string.Format("{0}C, {1}hPa ", Math.Round(bmp180.Temperature.DegreesCelsius, 1), Math.Round(bmp180.Pressure.Hectopascals, 0));
                strip.ScrollStringInFromRight(msg, 100, Glovebox.Graphics.BiColour.Green);
                await Task.Delay(10);
            }
        }

        async void ShowLight()
        {
            
            while (true)
            {
                string lightMsg = string.Format("{0}p ", Math.Round((1 - light.ReadRatio) * 100, 1));
                matrix.ScrollStringInFromRight(lightMsg, 100);
                await Task.Delay(10);
            }
        }

        private void Adapter_AllJoynGet(object sender, AllJoynAttributeData e)
        {
            if (e.name == "Message")
            {
                e.value = PropertyValue.CreateChar16Array(("hello world").ToCharArray());
            }
        }

        private void Adapter_AllJoynSet(object sender, AllJoynData e)
        {
            Debug.WriteLine(e.Value.Name);
        }
    }
}

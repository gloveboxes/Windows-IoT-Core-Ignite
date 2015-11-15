using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using BridgeRT;
using AdapterLib;
using Glovebox.Graphics.Components;
using Glovebox.IoT.Devices.Sensors;
using Glovebox.IoT.Devices.Converters;
using Glovebox.Graphics.Drivers;
using Microsoft.Maker.Media.UniversalMediaEngine;
using Windows.Media.SpeechSynthesis;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HeadlessAdapterApp
{
    public sealed class StartupTask : IBackgroundTask
    {

        LED8x8Matrix matrix = new LED8x8Matrix(new Ht16K33());
        LED8x8Matrix strip = new LED8x8Matrix(new MAX7219(4, MAX7219.Rotate.None, MAX7219.Transform.HorizontalFlip));

        BMP180 bmp180 = new BMP180();

        AdcProviderManager adcManager = new AdcProviderManager();
        private MediaEngine mediaEngine = new MediaEngine();
        SpeechSynthesizer synth;


        Ldr light = null;



        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            Adapter adapter = null;
            deferral = taskInstance.GetDeferral();



            matrix.SetBrightness(1);
            strip.SetBrightness(1);

            adcManager.Providers.Add(new ADS1015(ADS1015.Gain.Volt33));
            var ads1015 = (await adcManager.GetControllersAsync())[0];
            light = new Ldr(ads1015.OpenChannel(2));

            synth = new SpeechSynthesizer();
            var result = await mediaEngine.InitializeAsync();



            //var result = await mediaEngine.InitializeAsync();


            try
            {
                adapter = new Adapter();
                dsbBridge = new DsbBridge(adapter);

                var initResult = dsbBridge.Initialize();
                if (initResult != 0)
                {
                    throw new Exception("DSB Bridge initialization failed!");
                }
            }
            catch (Exception ex)
            {
                if (dsbBridge != null)
                {
                    dsbBridge.Shutdown();
                }

                if (adapter != null)
                {
                    adapter.Shutdown();
                }

                throw;
            }


            adapter.AllJoynMethod += Adapter_AllJoynMethod;


            ShowTempPressure();
            ShowLight();

        }


        private void Adapter_AllJoynMethod(object sender, AllJoynMethodData e)
        {
            if (e.Method.Name.ToLower() == "joke")
            {
                var p = e.AdapterDevice.Properties.Where(x => x.Name == "Speech").First()
                        .Attributes.Where(y => y.Value.Name == "Message").First();

                if (p != null) {Speak(p.Value.Data as string);}   
            }
        }

        private async void Speak(string message)
        {
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(message);

            mediaEngine.PlayStream(stream);
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

        private DsbBridge dsbBridge;
        private BackgroundTaskDeferral deferral;
    }
}

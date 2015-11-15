using Glovebox.Graphics.Components;
using Glovebox.Graphics.Drivers;
using Glovebox.IoT.Devices.Converters;
using Glovebox.IoT.Devices.Sensors;
using System;
using System.Threading.Tasks;

namespace HeadlessAdapterApp
{
    class Banner : AllJoyn
    {

        LED8x8Matrix matrix = new LED8x8Matrix(new Ht16K33());
        LED8x8Matrix strip = new LED8x8Matrix(new MAX7219(4, MAX7219.Rotate.None, MAX7219.Transform.HorizontalFlip));

        // sensors
        BMP180 bmp180 = new BMP180();
        Ldr light = null;

        //ADC
        AdcProviderManager adcManager = new AdcProviderManager();

        public Banner()
        {

            matrix.SetBrightness(1);
            strip.SetBrightness(3);

        }


        protected async Task InitBanner()
        {
            await InitAdc();
            ShowTempPressure();
            ShowLight();
        }

        private async Task InitAdc()
        {
            adcManager.Providers.Add(new ADS1015(ADS1015.Gain.Volt33));
            var ads1015 = (await adcManager.GetControllersAsync())[0];
            light = new Ldr(ads1015.OpenChannel(2));
        }

        async void ShowTempPressure()
        {
            while (true)
            {
                string msg = string.Format("{0}, {1}C, {2}hPa, {3} ", preMessage, Math.Round(bmp180.Temperature.DegreesCelsius, 1), Math.Round(bmp180.Pressure.Hectopascals, 0), postMessage);
                strip.ScrollStringInFromRight(msg, 80);
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
    }
}

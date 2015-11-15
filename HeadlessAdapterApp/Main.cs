using AdapterLib;
using Microsoft.Maker.Media.UniversalMediaEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace HeadlessAdapterApp
{
    class Main : Banner
    {
        //Speech Synth
        private MediaEngine mediaEngine = new MediaEngine();
        SpeechSynthesizer synth;



        public async Task<bool> Initialise()
        {
            synth = new SpeechSynthesizer();
            var result = await mediaEngine.InitializeAsync();


            InitAllJoyn();
            InitBanner();

            adapter.AllJoynMethod += Adapter_AllJoynMethod;

            return true;
        }

        public override void Adapter_AllJoynMethod(object sender, AllJoynMethodData e)
        {
            switch (e.Method.Name.ToLower())
            {

                case "joke":
                    var p = e.AdapterDevice.Properties.Where(x => x.Name == "Speech").First()
                       .Attributes.Where(y => y.Value.Name == "Message").First();

                    if (p != null) { Speak(p.Value.Data as string); }
                    break;

                default:
                    break;
            }

            base.Adapter_AllJoynMethod(sender, e);
        }

        private async void Speak(string message)
        {
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(message);

            mediaEngine.PlayStream(stream);
        }
    }
}

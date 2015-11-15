using Microsoft.Maker.Media.UniversalMediaEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace HeadlessAdapterApp
{
    class Speech : Banner
    {
        //Speech Synth
        private MediaEngine mediaEngine = new MediaEngine();
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public  Speech()
        {
            InitSpeech().Wait();
        }

        public async Task InitSpeech()
        {
            await mediaEngine.InitializeAsync();
        }

        protected async void Speak(string message)
        {
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(message);
            mediaEngine.PlayStream(stream);
        }
    }
}

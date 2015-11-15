using Microsoft.Maker.Media.UniversalMediaEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace HeadlessAdapterApp
{
    class Speech : Banner
    {
        //Speech Synth
        private MediaEngine mediaEngine;
        SpeechSynthesizer synth;

        public Speech()
        {
            InitSpeech().Wait();
        }

        public async Task InitSpeech()
        {
            mediaEngine = new MediaEngine();
            synth = new SpeechSynthesizer();
            await mediaEngine.InitializeAsync();
        }

        protected async void Speak(string message)
        {
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(message);
            mediaEngine.PlayStream(stream);
        }
    }
}

using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HeadlessAdapterApp
{
    public sealed class StartupTask : IBackgroundTask
    {

        private BackgroundTaskDeferral deferral;
        private Main _main = new Main();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            deferral = taskInstance.GetDeferral();
            await _main.Initialise();
        }
    }
}

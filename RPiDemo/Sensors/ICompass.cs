using System;
using System.Threading.Tasks;

namespace RPiDemo.Sensors
{
    public interface ICompass : IDisposable
    {
        event EventHandler<CompassReading> CompassReadingChangedEvent;

        bool IsInitialized { get; }

        MagnetometerMeasurementMode MeasurementMode { get; }

        Task SetModeAsync(MagnetometerMeasurementMode measurementMode);
    }
}

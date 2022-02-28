using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public interface IBandManagerService
    {
        BandStatus BandStatus { get;  set; }
        string BandName { get;  set; }
        AccelerometerSensor Accelerometer { get;  set; }
        GSRSensor Gsr { get; set; }
        GyroscopeSensor Gyroscope { get;  set; }
        HeartRateSensor HeartRate { get;  set; }
        TemperatureSensor Temperature { get;  set; }
        RRIntervalSensor RRInterval { get;  set; }

        Task ConnectBand(int selectedIndex, string bandName);
        Task SubscribeSensors();
        Task<IEnumerable<string>> GetPairedBands();
    }
}

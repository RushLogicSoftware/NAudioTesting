using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest.PlaybackTests
{
    public class WaveOutConfigOptions
    {
        public List<AudioDevice> Devices { get; set; }
    }
    public class AudioDevice
    {
        public int DeviceIndex { get; set; }
        public string Name { get; set; }
    }

    public class WaveOutPlaybackConfiguration
    {
        public AudioDevice selectedDevice { get; set; }
        public WaveOutPlaybackTest.eVolumeControlType VolumeControlType { get; set; }
        public int? NumberOfBuffers { get; set; }
        public int? DesiredLatency { get; set; }
        public float? InitialVolume { get; set; }
    }

}

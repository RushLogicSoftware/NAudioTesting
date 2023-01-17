using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest.RecordingTests
{
    public class WaveInConfigOptions
    {
        public List<InputDevice> Devices { get; set; }
    }
    public class InputDevice
    {
        public int DeviceIndex { get; set; }
        public string Name { get; set; }
        public int ChannelCount { get; set; }
    }

    public class WaveInRecordingConfiguration
    {
        public InputDevice selectedDevice { get; set; }
        //public WaveOutPlaybackTest.eVolumeControlType VolumeControlType { get; set; }
        public int? NumberOfBuffers { get; set; }
        public int? DesiredLatency { get; set; }
        public float? InitialVolume { get; set; }

        public eSampleRate sampleRate { get; set; }
        public eBitDepth bitDepth { get; set; }
        public List<int> SelectedChannels { get; set; }

    }
}

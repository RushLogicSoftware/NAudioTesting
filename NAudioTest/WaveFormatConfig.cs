using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace NAudioTest
{
    public enum eAudioFileType { MP3, WAV };
    public enum eSampleRate { _8kHz, _16kHz, _22_05kHz, _44_1kHz };
    public enum eBitDepth { _16_Bits, _24_Bits };

    public static class WaveFormatConfig
    {
        public static Dictionary<eSampleRate, int> SampleRates { get; set; }
        public static Dictionary<eBitDepth, int> BitDepths { get; set; }
        static WaveFormatConfig()
        {
            SampleRates = new Dictionary<eSampleRate, int>();
            SampleRates.Add(eSampleRate._8kHz, 8000);
            SampleRates.Add(eSampleRate._16kHz, 16000);
            SampleRates.Add(eSampleRate._22_05kHz, 22050);
            SampleRates.Add(eSampleRate._44_1kHz, 44100);

            BitDepths = new Dictionary<eBitDepth, int>();
            BitDepths.Add(eBitDepth._16_Bits, 16);
        }

        public static WaveFormat GetWaveFormat(eSampleRate sampleRate, eBitDepth bitDepth, int channelCount)
        {
            return GetWaveFormat(SampleRates[sampleRate], bitDepth, channelCount);
        }
        public static WaveFormat GetWaveFormat(int sampleRate, eBitDepth bitDepth, int channelCount)
        {
            WaveFormat format = new WaveFormat(sampleRate, BitDepths[bitDepth], channelCount);

            return format;
        }
    }
}

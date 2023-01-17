using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.Channels
{
    public class ChannelChanger
    {
        public void StereoToMono_Int16(string inputMp3FilePath, string outputWavFilePath)
        {
            // Mp3FileReader provides 16-bit samples, which is what StereoToMonoProvider16 requires.
            using (Mp3FileReader reader = new Mp3FileReader(inputMp3FilePath))
            {
                StereoToMonoProvider16 mono = new StereoToMonoProvider16(reader);
                mono.LeftVolume = 1.0f; // we will keep the left channel only
                mono.RightVolume = 0.0f;

                // write to .WAV
                WaveFileWriter.CreateWaveFile(outputWavFilePath, mono);
            }
        }

        public void StereoToMono_IEEEFloat(string inputMp3FilePath)
        {
            // AudioFileReader provides 32-bit IEEE Floating Point samples,
            // which is what our custom StereoToMonoSampleProvider requires.
            using (AudioFileReader reader = new AudioFileReader(inputMp3FilePath))
            {
                StereoToMonoSampleProvider mono = new StereoToMonoSampleProvider(reader);
                mono.LeftVolume = 1.0f; // we will keep the left channel only
                mono.RightVolume = 0.0f;

                throw new Exception("output to file is not yet implemented.");
            }
        }

    }
}

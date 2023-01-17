using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace NAudioTest.FormatConversions
{
    public class FileFormatConverter
    {
        public void MP3ToWAV()
        {
            // open a reader for the incoming .MP3 file data
            using (var reader = new Mp3FileReader(AudioTestingPaths.LongAudioFile))
            {
                // write all contents from the reader (which are PCM samples) to uncompressed PCM .wav.
                WaveFileWriter.CreateWaveFile(AudioTestingPaths.NAudioTestingDirPath + "MP3ToWAV.wav", reader);
            }
        }

        public void VideoToWAV()
        {
            // NOTE: AudioFileReader couldve been used here! it can still read video. since that would return
            //       IEEE floating point samples (which are large), you could use CreateWaveFile16() to convert
            //       those samples back to 16-bit integer samples.

            // open a reader for the incoming video file data
            using (var reader = new MediaFoundationReader(AudioTestingPaths.VideoFilePath))
            {
                // write all contents from the reader (which are PCM samples) to uncompressed PCM .wav.
                WaveFileWriter.CreateWaveFile(AudioTestingPaths.NAudioTestingDirPath + "VideoToWAV.wav", reader);
            }
        }
    }
}

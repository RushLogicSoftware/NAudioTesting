using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;

namespace NAudioTest.ReadingFormatDetails
{
    public class AudioFormatDetailReader
    {
        public class AudioFileFormatDetails
        {
            public int AverageBytesPerSecond { get; set; }
            public int SampleRate { get; set; }
            public int BitsPerSample { get; set; }
            public WaveFormatEncoding Encoding { get; set; }
            public int ChannelCount { get; set; }
        }

        /// <summary>
        /// IS THIS CORRECT? even if the file were in a different format, are we getting whatever WaveFormat is provided by the reader??
        /// </summary>
        public AudioFileFormatDetails GetAudioFileFormatDetails(string sourcePath)
        {
            AudioFileFormatDetails details = null;

            switch (new FileInfo(sourcePath).Extension.ToLower())
            {
                case ".wav":
                    using (var reader = new WaveFileReader(sourcePath))
                    {
                        details = PopulateAudioFileFormatDetails(reader);
                    }
                    break;
                case ".mp3":
                    using (var reader = new Mp3FileReader(sourcePath))
                    {
                        details = PopulateAudioFileFormatDetails(reader);
                    }
                    break;
            }
            return details;
        }
        AudioFileFormatDetails PopulateAudioFileFormatDetails(WaveStream reader)
        {
            WaveFormat waveFormat = reader.WaveFormat;
            AudioFileFormatDetails details = new AudioFileFormatDetails {
                AverageBytesPerSecond = waveFormat.AverageBytesPerSecond,
                SampleRate = waveFormat.SampleRate,
                BitsPerSample = waveFormat.BitsPerSample,
                Encoding = waveFormat.Encoding,
                ChannelCount = waveFormat.Channels,
            };
            return details;
        }



    }
}

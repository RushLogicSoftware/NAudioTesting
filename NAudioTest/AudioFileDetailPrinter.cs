using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using NAudioTest.ReadingFormatDetails;

namespace NAudioTest
{
    public static class AudioFileDetailPrinter
    {
        public static void PrintFileDetails(string audioFilePath)
        {
            AudioFormatDetailReader detailReader = new AudioFormatDetailReader();
            AudioFormatDetailReader.AudioFileFormatDetails details = detailReader.GetAudioFileFormatDetails(audioFilePath);
            Console.WriteLine("audio file details:");
            Console.WriteLine("    FilePath: " + audioFilePath);
            Console.WriteLine("    AverageBytesPerSecond: " + details.AverageBytesPerSecond);
            Console.WriteLine("    SampleRate: " + details.SampleRate);
            Console.WriteLine("    BitsPerSample: " + details.BitsPerSample);
            Console.WriteLine("    Encoding: " + details.Encoding);
            Console.WriteLine("    ChannelCount: " + details.ChannelCount);
        }
    }
}

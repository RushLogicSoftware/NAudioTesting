using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using NAudio.MediaFoundation;
using NAudio.Wave;

using NAudioTest.PlaybackTests;
using NAudioTest.EditingAudio;
using NAudioTest.FormatConversions;
using NAudioTest.SynthesizingSamples;
using NAudioTest.ReadingFormatDetails;
using NAudioTest.Resampling;
using NAudioTest.BitDepthConversion;
using NAudioTest.Channels;
using NAudioTest.Codecs.GSM;
using NAudioTest.Codecs.MFT;
using NAudioTest.Codecs.LAME;
using NAudioTest.Mixing;
using NAudioTest.RecordingTests;
using NAudioTest.Asio;

namespace NAudioTest._TestOptionSelection
{
    public static class ChannelConversionTestOptions
    {
        public static void TestChannelConversion()
        {
            string mp3Path = AudioTestingPaths.MediumAudioFile;
            string outputWavFilePath = null;
            ChannelChanger channelChanger = new ChannelChanger();
            Console.WriteLine("testing Stereo to Mono conversion.");
            Console.WriteLine("sample formats to test:");
            Console.WriteLine("    1.) 16-bit integer samples");
            Console.WriteLine("    2.) 32-bit IEEE Floating Point DSP samples");
            Console.WriteLine("enter the number corresponding to the type of samples you want to test channel conversion with:");
            switch (Int32.Parse(Console.ReadLine()))
            {
                case 1: // 16-bit integer samples
                    outputWavFilePath = AudioTestingPaths.NAudioTestingDirPath + "StereoToMono_Int16.wav";
                    channelChanger.StereoToMono_Int16(mp3Path, outputWavFilePath);
                    break;
                case 2: // 32-bit IEEE Floating Point DSP samples
                    channelChanger.StereoToMono_IEEEFloat(mp3Path);
                    break;
            }

            Console.WriteLine("output .WAV file format:");
            AudioFileDetailPrinter.PrintFileDetails(outputWavFilePath);
        }
    }
}

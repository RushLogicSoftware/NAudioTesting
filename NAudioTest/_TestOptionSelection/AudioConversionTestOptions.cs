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
    public static class AudioConversionTestOptions
    {
        public static void TestAudioConversion()
        {
            FileFormatConverter converter = new FileFormatConverter();

            Console.WriteLine("supported formats are:");
            Console.WriteLine("    MP3");
            Console.WriteLine("    WAV");
            Console.WriteLine("    MP4 (this is a video)");
            Console.WriteLine("enter the source format: ");
            string sourceFormat = Console.ReadLine().ToUpper();
            Console.WriteLine("enter the destination format: ");
            string destinationFormat = Console.ReadLine().ToUpper();

            Console.WriteLine("performing format conversion");
            if (sourceFormat == "MP3" && destinationFormat == "WAV")
                converter.MP3ToWAV();
            else if (sourceFormat == "MP4" && destinationFormat == "WAV")
                converter.VideoToWAV();
            else throw new NotImplementedException(
                "conversion from " + sourceFormat + " to " + destinationFormat + " is not implemented."
                );
        }
    }
}

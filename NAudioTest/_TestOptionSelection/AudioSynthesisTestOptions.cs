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
    public static class AudioSynthesisTestOptions
    {
        public static void TestAudioSynthesis()
        {
            PCMWavSynthTest synthTest = new PCMWavSynthTest();
            Console.WriteLine("how do you want to synthesize the audio? ways to test audio synthesis include:");
            Console.WriteLine("    mathematical = uses raw mathematical logic to generate a tone.");
            Console.WriteLine("    SignalGenerator = uses NAudio SignalGenerator to generate a tone.");

            switch (Console.ReadLine().ToLower())
            {
                case "mathematical":
                    synthTest.WaveSound_MathematicalSynthesis(eSampleRate._16kHz, eBitDepth._16_Bits, 1);
                    break;
                case "signalgenerator":
                    Console.WriteLine("would you like to use the better, signal chain strategy? enter y or n.");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        eBitDepth outputBitDepth = BitDepthSelector.SelectBitDepth();
                        string outputFilePath = AudioTestingPaths.NAudioTestingDirPath + "SignalGenerator_" + outputBitDepth.ToString().Replace("_", "") + ".wav";
                        synthTest.WaveSound_SignalGenerator_SignalChain(eSampleRate._16kHz, outputBitDepth, outputFilePath);
                        Console.WriteLine("synthesized wave file...");
                        AudioFileDetailPrinter.PrintFileDetails(outputFilePath);
                    }
                    else
                        synthTest.WaveSound_SignalGenerator(eSampleRate._16kHz);
                    break;
            }
        }
    }
}

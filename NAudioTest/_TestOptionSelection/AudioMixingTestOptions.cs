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
    public static class AudioMixingTestOptions
    {
        public static void TestAudioMixing()
        {
            // CRITICALLY IMPORTANT!!!!!!!!!!
            // remember that all sample rates and channel counts must be identical!!!!
            // either make sure that all possible inputs are at the same sample rate and channel count, and initialize the mixer with this,
            // or implement resampling and channel conversion in the signal chain.
            int sampleRate = 48000;
            int channelCount = 2;


            RealTimeEndlessMixingTest endlessMixer = new RealTimeEndlessMixingTest(sampleRate, channelCount);
            int inputCount = 0;
            List<string> inputs = new List<string>();
            bool mixing = true;
            while (mixing)
            {
                Console.WriteLine("what operation would you like to perform on the mixer:");
                Console.WriteLine("    add_input = add an audio input");
                Console.WriteLine("    remove_input = add an audio input");
                Console.WriteLine("    quit");
                switch (Console.ReadLine().ToLower())
                {
                    case "add_input":
                        string audioFilePath = null;
                        inputCount++;
                        switch (inputCount)
                        {
                            case 1:
                                audioFilePath = AudioTestingPaths.VoiceQuote_1;
                                break;
                            case 2:
                                audioFilePath = AudioTestingPaths.VoiceQuote_2;
                                break;
                            case 3: throw new NotImplementedException("only 2 inputs implemented at the moment.");
                        }
                        endlessMixer.PlaySound(inputCount.ToString(), audioFilePath);
                        break;
                    case "remove_input":
                        endlessMixer.StopSound(inputCount.ToString());
                        inputCount--;
                        break;
                    case "quit":
                        endlessMixer.StopMixer();
                        mixing = false;
                        break;
                }
            }
        }
    }
}

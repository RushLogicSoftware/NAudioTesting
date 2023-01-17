using System;

using NAudioTest.BitDepthConversion;
using NAudioTest._TestOptionSelection;

namespace NAudioTest
{
    /*
     * Author: Gavan Rush
     * Date: 9/16/2021
     */
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("========================================================================================");
            Console.WriteLine("========================================================================================");
            Console.WriteLine("this program demonstrates a number of the audio capabilities available in NAudio.");
            Console.WriteLine("it can be used as a companion application for the following PluralSight courses:");
            Console.WriteLine("     1.) Digital Audio Fundamentals");
            Console.WriteLine("     2.) Audio Programming with NAudio");
            Console.WriteLine();
            Console.WriteLine("edit the App.config file to specify a local audio testing directory and some audio");
            Console.WriteLine("and video filenames you intend to use for testing.");
            Console.WriteLine();
            Console.WriteLine("press <ENTER> to begin testing NAudio functionality.");
            Console.WriteLine();
            Console.WriteLine("~ Gavan Rush");
            Console.WriteLine("========================================================================================");
            Console.WriteLine("========================================================================================");
            Console.ReadLine();


            Console.Clear();
            bool testing = true;
            while (testing)
            {
                Console.WriteLine("testing NAudio. you can test the following features:");
                Console.WriteLine("    playback = test different audio playback techniques");
                Console.WriteLine("    details = load & display details about audio files");
                Console.WriteLine("    extract = test extracting a piece of audio to a separate file.");
                Console.WriteLine("    convert = test conversions between different audio file formats.");
                Console.WriteLine("    synthesize = test synthesizing audio.");
                Console.WriteLine("    concatinate = test concatinating audio files.");
                Console.WriteLine("    resample = test resampling.");
                Console.WriteLine("    bitdepth = test working with bit depths.");
                Console.WriteLine("    channels = test changing channels.");
                Console.WriteLine("    codecs = test codec functionality.");
                Console.WriteLine("    mixing = test mixing.");
                Console.WriteLine("    recording = test recording.");
                Console.WriteLine("    q = quit.");
                Console.WriteLine();
                Console.WriteLine("enter one of the feature names listed above, then press <ENTER>.");
                string userInput = Console.ReadLine().ToLower();
                Console.Clear();
                switch (userInput)
                {
                    case "playback":
                        PlaybackTestOptions.TestPlayback();
                        break;
                    case "details":
                        AudioFileDetailTestOptions.TestAudioFileDetailPrinting();
                        break;
                    case "extract":
                        AudioExtractionTestOptions.TestAudioExtraction();
                        break;
                    case "convert":
                        AudioConversionTestOptions.TestAudioConversion();
                        break;
                    case "synthesize":
                        AudioSynthesisTestOptions.TestAudioSynthesis();
                        break;
                    case "concatinate":
                        AudioConcatenationTestOptions.TestAudioFileConcatenation();
                        break;
                    case "resample":
                        ResamplingTestOptions.TestResampling();
                        break;
                    case "bitdepth":
                        BitDepthConverter depthConverter = new BitDepthConverter();
                        depthConverter.FloatingPointAndBack(AudioTestingPaths.MediumAudioFile);
                        break;
                    case "channels":
                        ChannelConversionTestOptions.TestChannelConversion();
                        break;
                    case "codecs":
                        CodecTestOptions.TestCodecs();
                        break;
                    case "mixing":
                        AudioMixingTestOptions.TestAudioMixing();
                        break;
                    case "recording":
                        RecordingTestOptions.TestRecording();
                        break;
                    case "q":
                        testing = false;
                        break;
                }
                Console.Clear();
            }
        }
    }
}

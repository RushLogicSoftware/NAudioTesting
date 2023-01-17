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
    public static class CodecTestOptions
    {
        public static void TestCodecs()
        {
            Console.WriteLine("ready to test codec functionality.");
            Console.WriteLine("what type of codec would you like to test?");
            Console.WriteLine("    GSM = test GSM codec functionality");
            Console.WriteLine("    MFT = test MFT API functionality");
            Console.WriteLine("    lame = test the lame command line utility.");
            switch (Console.ReadLine().ToLower())
            {
                case "gsm":
                    #region GSM
                    {
                        GSMTesting gsmTesting = new GSMTesting();
                        Console.WriteLine("    what type of gsm test would you like to run?");
                        Console.WriteLine("        PCM_to_GSM = convert from PCM audio to GSM.");
                        switch (Console.ReadLine().ToLower())
                        {
                            case "pcm_to_gsm":
                                #region pcm_to_gsm
                                {
                                    throw new NotImplementedException(
                                        "need to either get a properly formatted input file or enhance the test to do the conversion. " +
                                        "also need to implement conversion from GSM back to PCM. this needs to be implemented & tested."
                                        );
                                    string inputFile = AudioTestingPaths.VoiceQuote_4;
                                    string outputFile = AudioTestingPaths.NAudioTestingDirPath + "PCM_to_GSM-6.10.wav";
                                    Console.WriteLine("input PCM WAV file: " + inputFile);
                                    AudioFileDetailPrinter.PrintFileDetails(inputFile);
                                    gsmTesting.Convert_PCM_to_GSM610(inputFile, outputFile);
                                }
                                break;
                                #endregion
                        }
                    }
                    break;
                    #endregion
                case "mft":
                    #region MFT
                    {
                        MftAacTesting mftAacTesting = new MftAacTesting();
                        Console.WriteLine("    what MFT operation would you like to run?");
                        Console.WriteLine("        MediaTypes = list available media types.");
                        Console.WriteLine("        MP3_to_AAC = convert from MP3 audio to AAC.");
                        switch (Console.ReadLine().ToLower())
                        {
                            case "mediatypes":
                                #region MediaTypes
                                {
                                    Console.WriteLine("what audio subtype do you want to see media types for?");
                                    Console.WriteLine("    MP3");
                                    Console.WriteLine("    PCM");
                                    eMFTAudioSubtype audioSubtype = eMFTAudioSubtype.MFAudioFormat_MP3;
                                    switch (Console.ReadLine().ToLower())
                                    {
                                        case "mp3": audioSubtype = eMFTAudioSubtype.MFAudioFormat_MP3; break;
                                        case "pcm": audioSubtype = eMFTAudioSubtype.MFAudioFormat_PCM; break;
                                    }

                                    Console.WriteLine("==== available media types ==============================");
                                    MftCapabilityChecker capabilityChecker = new MftCapabilityChecker();

                                    List<MftCapabilityChecker.MediaTypeWrapper> outputMediaTypes;
                                    switch (capabilityChecker.GetAvailableOutputMediaTypesForAudioSubtype(audioSubtype, out outputMediaTypes))
                                    {
                                        case eAvailableOutputMediaTypeQueryResult.TRANSFORMS_FOUND:
                                            outputMediaTypes.ForEach(mediaType => MediaTypeDetailPrinter.PrintMediaTypeDetails(mediaType));
                                            break;
                                        case eAvailableOutputMediaTypeQueryResult.NO_SUITABLE_TRANSFORM_FOUND:
                                            Console.WriteLine("No suitable transforms found. MFT must not have the necessary Codecs installed.");
                                            break;
                                        case eAvailableOutputMediaTypeQueryResult.UNKNOWN:
                                            throw new Exception("an unknown exception has occurred");
                                            break;
                                    }

                                    Console.WriteLine("=========================================================");
                                }
                                break;
                                #endregion
                            case "mp3_to_aac":
                                #region MP3_to_AAC
                                {
                                    string inputFile = AudioTestingPaths.MediumAudioFile;

                                    // get formatting details about the input file
                                    AudioFormatDetailReader.AudioFileFormatDetails details = new AudioFormatDetailReader().GetAudioFileFormatDetails(inputFile);
                                    eMFTAudioSubtype inputAudioSubtype = eMFTAudioSubtype.MFAudioFormat_MP3;
                                    int inputSampleRate = details.SampleRate;
                                    int inputChannelCount = details.ChannelCount;

                                    // OPTIONAL: select one of the supported bitrates that MFT can actually use for this format.
                                    int preferredBitrate = SelectMFTBitrate(inputAudioSubtype, inputSampleRate, inputChannelCount);

                                    // do the conversion
                                    string outputFile = AudioTestingPaths.NAudioTestingDirPath + "MP3_to_AAC_" + preferredBitrate + "Hz.aac";
                                    mftAacTesting.Convert_MP3_to_AAC(inputFile, outputFile, preferredBitrate);
                                }
                                break;
                                #endregion
                        }
                    }
                    break;
                #endregion
                case "lame":
                    #region lame
                    {
                        LameTest lameTest = new LameTest();
                        Console.WriteLine("what input file format do you want?");
                        Console.WriteLine("    MP3");
                        Console.WriteLine("    WAV");
                        string inputFile = null;
                        string inputFormat = Console.ReadLine().ToLower();
                        switch (inputFormat)
                        {
                            case "mp3":
                                inputFile = AudioTestingPaths.MediumAudioFile;
                                break;
                            case "wav":
                                inputFile = AudioTestingPaths.VoiceQuote_2;
                                break;
                        }
                        Console.WriteLine("what output file format do you want?");
                        Console.WriteLine("    MP3");
                        Console.WriteLine("    WAV");
                        string outputFormat = Console.ReadLine().ToLower();
                        Console.WriteLine("what bit depth do you want?");
                        int bitRate_kbps = Int32.Parse(Console.ReadLine());
                        string outputFilePath = AudioTestingPaths.NAudioTestingDirPath + "LameConversion_" + inputFormat + "_to_" + outputFormat + "." + outputFormat;

                        Console.WriteLine("input file:");
                        AudioFileDetailPrinter.PrintFileDetails(inputFile);

                        lameTest.LameConversion(inputFile, outputFilePath, bitRate_kbps);

                        Console.WriteLine("output file:");
                        AudioFileDetailPrinter.PrintFileDetails(outputFilePath);
                    }
                    break;
                    #endregion
            }
        }
        static int SelectMFTBitrate(eMFTAudioSubtype audioSubtype, int sampleRate, int channelCount)
        {
            // iterate through all supported bitrates for this format
            Dictionary<int, int> printedBitrates = new Dictionary<int, int>();
            int printNum = 1;
            new MftCapabilityChecker().GetAvailableBitratesForAudioSubtype(audioSubtype, sampleRate, channelCount).ToList()
                .ForEach(bitRate => {
                    printedBitrates.Add(printNum, bitRate);
                    Console.WriteLine("    " + printNum + ".) " + bitRate);
                    printNum++;
                });

            // let the user select a bitrate
            Console.WriteLine("enter the number corresponding to the bitrate you want to select:");
            return printedBitrates[Int32.Parse(Console.ReadLine())];
        }
    }
}

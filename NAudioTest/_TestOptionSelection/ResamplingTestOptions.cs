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
    public static class ResamplingTestOptions
    {
        public static void TestResampling()
        {
            #region choose input file based on its extension
            Console.WriteLine("what file format would you like to input for resampling?");
            Console.WriteLine("    WAV");
            Console.WriteLine("    MP3");
            eAudioFileType fileType = eAudioFileType.WAV;
            string audioFilePath = null;
            switch (Console.ReadLine().ToLower())
            {
                case "wav":
                    audioFilePath = AudioTestingPaths.VoiceQuote_2;
                    fileType = eAudioFileType.WAV;
                    break;
                case "mp3":
                    audioFilePath = AudioTestingPaths.MediumAudioFile;
                    fileType = eAudioFileType.MP3;
                    break;
            }
            #endregion

            #region choose resampling algorithm
            Console.WriteLine("available resampling algorithms");
            Dictionary<int, eResamplingAlgorithm> printedAlgorithms = new Dictionary<int, eResamplingAlgorithm>();
            int printNum = 1;
            Enum.GetValues(typeof(eResamplingAlgorithm)).OfType<eResamplingAlgorithm>().ToList().ForEach(alg => {
                printedAlgorithms.Add(printNum, alg);
                Console.WriteLine("    " + printNum + ".) " + alg);
                printNum++;
            });
            Console.WriteLine("enter the number corresponding to the resampling algorithm you would like to use: ");
            eResamplingAlgorithm algorithm = printedAlgorithms[Int32.Parse(Console.ReadLine())];
            #endregion


            int ResamplerQuality_1_to_60 = 0;
            if (algorithm == eResamplingAlgorithm.MFT)
            {
                Console.WriteLine("what resampling quality do you want to use? enter a value 1 to 60: ");
                ResamplerQuality_1_to_60 = Int32.Parse(Console.ReadLine());
            }


            string outputFilePath = AudioTestingPaths.NAudioTestingDirPath + "Resampled_" + algorithm + 
                (algorithm == eResamplingAlgorithm.MFT ? ("_quality" + ResamplerQuality_1_to_60) : "") +
                ".wav"
                ;

            AudioFormatDetailReader detailReader = new AudioFormatDetailReader();
            if (fileType == eAudioFileType.WAV)
            {
                AudioFileDetailPrinter.PrintFileDetails(audioFilePath);
            }
            else Console.WriteLine("TODO: implement AudioFormatDetailReader to distinguish between different file types.");


            ResamplingTest resampler = new ResamplingTest();
            resampler.MFTResample(audioFilePath, outputFilePath, eSampleRate._16kHz, algorithm, ResamplerQuality_1_to_60);


            if (fileType == eAudioFileType.WAV)
            {
                AudioFileDetailPrinter.PrintFileDetails(outputFilePath);
            }
            else Console.WriteLine("TODO: implement AudioFormatDetailReader to distinguish between different file types.");
        }
    }
}

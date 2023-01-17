using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;
using NAudioTest.ReadingFormatDetails;
using NAudioTest.SynthesizingSamples;

namespace NAudioTest.Resampling
{
    public enum eResamplingAlgorithm { ACM, MFT };
    public class ResamplingTest
    {
        public void MFTResample(string inputAudioFilePath, string outputAudioFilePath, eSampleRate newSampleRate, eResamplingAlgorithm resamplingAlgorithm, int ResamplerQuality_1_to_60 = 60)
        {
            string extension = new FileInfo(inputAudioFilePath).Extension.ToLower();
            switch (extension)
            {
                case ".wav":
                    using (var reader = new WaveFileReader(inputAudioFilePath))
                    {
                        UseCorrectResamplingAlgorithm(reader, outputAudioFilePath, newSampleRate, resamplingAlgorithm, ResamplerQuality_1_to_60);
                    }
                    break;
                case ".mp3":
                    using (var reader = new Mp3FileReader(inputAudioFilePath))
                    {
                        UseCorrectResamplingAlgorithm(reader, outputAudioFilePath, newSampleRate, resamplingAlgorithm, ResamplerQuality_1_to_60);
                    }
                    break;
                default: throw new NotImplementedException("resampling is not yet implemented for files with extension: " + extension);
            }
        }

        void UseCorrectResamplingAlgorithm(WaveStream reader, string outputAudioFilePath, eSampleRate newSampleRate, eResamplingAlgorithm resamplingAlgorithm, int ResamplerQuality_1_to_60 = 30)
        {
            WaveFormat inputFileFormat = reader.WaveFormat;

            // resample to a new file
            switch (resamplingAlgorithm)
            {
                case eResamplingAlgorithm.ACM:
                    OutputResampledAudio_ACM(outputAudioFilePath, reader, newSampleRate, inputFileFormat.BitsPerSample, inputFileFormat.Channels);
                    break;
                case eResamplingAlgorithm.MFT:
                    OutputResampledAudio_MFT(outputAudioFilePath, reader, newSampleRate, ResamplerQuality_1_to_60);
                    break;
                default: throw new NotImplementedException("resampling algorithm " + resamplingAlgorithm + " is not yet implemented.");
            }
        }


        #region resamplers
        void OutputResampledAudio_ACM(string outputFilePath, WaveStream sourceStream, eSampleRate newSampleRate, int bitsPerSample, int channelCount)
        {
            List<eBitDepth> bitDepths = WaveFormatConfig.BitDepths.Where(kvp => kvp.Value == bitsPerSample).Select(kvp => kvp.Key).ToList();
            if (bitDepths.Count != 1) throw new NotImplementedException("bit depth " + bitsPerSample + " is not fully implemented.");

            eBitDepth bitDepth = bitDepths.Single();
            int newSampleRate_Int = WaveFormatConfig.SampleRates[newSampleRate];

            WaveFormat resampledWaveFormat = WaveFormatConfig.GetWaveFormat(newSampleRate, bitDepth, channelCount);
            using (var resampler = new WaveFormatConversionStream(resampledWaveFormat, sourceStream))
            {
                WaveFileWriter.CreateWaveFile(outputFilePath, resampler);
            }
        }
        void OutputResampledAudio_MFT(string outputFilePath, IWaveProvider sourceProvider, eSampleRate newSampleRate, int ResamplerQuality_1_to_60)
        {
            int newSampleRate_Int = WaveFormatConfig.SampleRates[newSampleRate];
            using (var resampler = new MediaFoundationResampler(sourceProvider, newSampleRate_Int))
            {
                resampler.ResamplerQuality = ResamplerQuality_1_to_60;
                WaveFileWriter.CreateWaveFile(outputFilePath, resampler);
            }
        }
        #endregion


    }
}

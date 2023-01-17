using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudioTest.BitDepthConversion;

namespace NAudioTest.SynthesizingSamples
{
    public class PCMWavSynthTest
    {
        /// <summary>
        /// uses a Sine wave to synthesize a tone.
        /// </summary>
        public void WaveSound_MathematicalSynthesis(eSampleRate sampleRate, eBitDepth bitDepth, int channelCount)
        {
            WaveFormat format = WaveFormatConfig.GetWaveFormat(sampleRate, bitDepth, channelCount);
            using (var writer = new WaveFileWriter(AudioTestingPaths.NAudioTestingDirPath + "MathematicalSynthesis.wav", format))
            {
                // construct an array of bytes which represents roughly 3 seconds of audio.
                int numBytesIn3Seconds = format.AverageBytesPerSecond * 3;
                byte[] samples = new byte[numBytesIn3Seconds];
                // synthesize some audio data
                for (int a = 0; a < numBytesIn3Seconds; a++)
                {
                    samples[a] = (byte)(Math.Sin(a * .04) * 15);
                }
                // write the sampels to the file
                writer.Write(samples, 0, samples.Length);
            }
        }

        /// <summary>
        /// uses NAudio built-in SignalGenerator to generate the tone.
        /// </summary>
        public void WaveSound_SignalGenerator(eSampleRate sampleRate)
        {
            int channelCount = 1;

            // SignalGenerator generates IEEE floating point samples. it is endless.
            // configure SignalGenerator to generate the sort of tone we want.
            int sampleRateInt = WaveFormatConfig.SampleRates[sampleRate];
            SignalGenerator generator = new SignalGenerator(sampleRateInt, channelCount);
            generator.Type = SignalGeneratorType.Sin;
            generator.Frequency = 1000; // 1kHz
            generator.Gain = 0.25;

            // use SignalGenerator to generate 10 seconds of tone and write to the output file.
            WaveFormat waveFormat = WaveFormatConfig.GetWaveFormat(sampleRate, eBitDepth._16_Bits, channelCount);
            using (var writer = new WaveFileWriter(AudioTestingPaths.NAudioTestingDirPath + "SignalGenerator.wav", waveFormat))
            {
                int samplesPerSecond = sampleRateInt * channelCount;
                // prepare a floating point buffer, because SignalGenerator will return IEEE float.
                float[] oneSecondBuffer = new float[samplesPerSecond];
                for (int a = 0; a < 10; a++)
                {
                    int samplesRead = generator.Read(oneSecondBuffer, 0, samplesPerSecond);
                    // because the writer has a WaveFormat indicating 16-bit samples, it will automatically convert the
                    // incoming IEEE floating point sampels to 16-bit integers before writing to the file.
                    writer.WriteSamples(oneSecondBuffer, 0, samplesRead);
                }
            }
        }


        /// <summary>
        /// uses SignalGenerator in a signal chain to generate the tone.
        /// </summary>
        public void WaveSound_SignalGenerator_SignalChain(eSampleRate sampleRate, eBitDepth outputBitDepth, string outputFilePath)
        {
            int channelCount = 1;
            int duration_seconds = 3;

            // SignalGenerator generates IEEE floating point samples. it is endless.
            // configure SignalGenerator to generate the sort of tone we want.
            int sampleRateInt = WaveFormatConfig.SampleRates[sampleRate];
            SignalGenerator generator = new SignalGenerator(sampleRateInt, channelCount);
            generator.Type = SignalGeneratorType.Sin;
            generator.Frequency = 1000; // 1kHz
            generator.Gain = 0.25;

            // use OffsetSampleProvider to decide where in the generated signal we want data from.
            // we can skip parts, create silence in the beginning, or take specific parts of the generated signal.
            // we use it to wrap the endless SignalGenerator with the something that has an end.
            OffsetSampleProvider offsetProvider = new OffsetSampleProvider(generator);
            int maxNumSamplesToRead = sampleRateInt * channelCount * duration_seconds;
            offsetProvider.TakeSamples = maxNumSamplesToRead; // setting TakeSamples to some max number means this provider has an end.

            // OffsetSampleProvider is not endless, so we can now move on and actually output the audio.

            // output the audio at the appropriate bit depth.
            switch (outputBitDepth)
            {
                case eBitDepth._16_Bits:
                    // use WaveFileWriter.CreateWaveFile16() to convert the IEEE floating point samples into 16-bit integer PCM.
                    WaveFileWriter.CreateWaveFile16(outputFilePath, offsetProvider);
                    break;
                case eBitDepth._24_Bits:
                    // use our custom IEEEFloatToWaveProvider24 to convert the IEEE floating point samples into 24-bit integer PCM.
                    IEEEFloatToWaveProvider24 floatTo24BitPCM = new IEEEFloatToWaveProvider24(offsetProvider);
                    WaveFileWriter.CreateWaveFile(outputFilePath, floatTo24BitPCM);
                    break;
                default: throw new NotImplementedException("bit depth " + outputBitDepth.ToString() + " is not yet implemented.");
            }
        }

    }
}

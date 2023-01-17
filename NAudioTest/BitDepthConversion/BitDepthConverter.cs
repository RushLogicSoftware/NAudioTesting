using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.BitDepthConversion
{
    /// <summary>
    /// NOTE FROM MARK HEATH: for high-performance in real-time applications, dont create so many buffers to be used
    ///                       temporarily. this creates extra work for the garbage collector. instead, the code could
    ///                       be optimized by allowing the caller to pass in the buffers to support reuse.
    /// </summary>
    public class BitDepthConverter
    {
        public void FloatingPointAndBack(string inputAudioFilePath)
        {
            // open a reader, which will provide 16-bit PCM samples
            string extension = new FileInfo(inputAudioFilePath).Extension.ToLower();
            switch (extension)
            {
                case ".mp3":
                    using (var reader = new Mp3FileReader(inputAudioFilePath))
                    {
                        // you can inspect and verify the expected bit depth and encoding...
                        WaveFormat inputFormat = reader.WaveFormat;
                        int BitsPerSample = inputFormat.BitsPerSample; // <-- this should be 16
                        WaveFormatEncoding Encoding = inputFormat.Encoding; // <-- this should be Pcm

                        // convert to 32-bit IEEE Floating Point samples.
                        ISampleProvider floatSampleProvider = PCM16_to_FloatingPoint32(reader);

                        // DO WORK HERE... this is your opportunity to do some floating point DSP.

                        // convert back to 16-bit PCM.
                        IWaveProvider pcmWaveProvider = FloatingPoint32_to_PCM16(floatSampleProvider);
                    }
                    break;
                default: throw new NotImplementedException("bit depth conversion is not yet implemented for extension: " + extension);
            }
        }

        public ISampleProvider PCM16_to_FloatingPoint32(WaveStream inputStream)
        {
            // verify the expected format of the input stream
            WaveFormat inputFormat = inputStream.WaveFormat;
            if (inputFormat.BitsPerSample != 16 || inputFormat.Encoding != WaveFormatEncoding.Pcm)
                throw new Exception("expected an input stream containing 16-bit PCM data, but received " + inputFormat.BitsPerSample + "-bit " + inputFormat.Encoding + ".");

            // create & return a sample provider which will provide 32-bit IEEE Floating Point samples.
            //  ** there should also be an extension method allowing you to do reader.ToSampleProvider()
            ISampleProvider floatSampleProvider = new Pcm16BitToSampleProvider(inputStream);
            return floatSampleProvider;
        }
        public IWaveProvider FloatingPoint32_to_PCM16(ISampleProvider floatSampleProvider)
        {
            // verify the expected format of the input
            WaveFormat inputFormat = floatSampleProvider.WaveFormat;
            if (inputFormat.BitsPerSample != 32 || inputFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                throw new Exception("expected an input stream containing 32-bit IeeeFloat data, but received " + inputFormat.BitsPerSample + "-bit " + inputFormat.Encoding + ".");

            // create & return a PCM wave provider which will provide 16-bit PCM samples.
            IWaveProvider pcmWaveProvider = new SampleToWaveProvider16(floatSampleProvider);
            return pcmWaveProvider;
        }



        #region sample conversions

        /// <summary>
        /// takes a byte[] buffer known to contain 16-bit integer samples & converts to 16-bit shorts.
        /// ** IMPORTANT: the incoming data is already 16-bit, but the bits of each sample are just spread over 2 bytes.
        /// </summary>
        public short[] Buffer16BitIntSamples_to_ShortArray16(byte[] input)
        {
            short[] outBuffer = new short[input.Length / 2];
            Buffer.BlockCopy(input, 0, outBuffer, 0, input.Length);
            return outBuffer;
        }

        /// <summary>
        /// takes a byte[] buffer known to contain 32-bit IEEE Floating Point samples & converts to an array of floats.
        /// ** IMPORTANT: the incoming data is already floating points, but the bits of each sample are just spread over 4 bytes.
        /// </summary>
        public float[] Buffer32BitFloatSamples_to_FloatArray(byte[] input)
        {
            float[] outBuffer = new float[input.Length / 4];
            Buffer.BlockCopy(input, 0, outBuffer, 0, input.Length);
            return outBuffer;
        }

        /// <summary>
        /// takes a byte[] buffer known to contain 16-bit integer samples & converts to an array of IEEE Floating Point samples.
        /// ** IMPORTANT: this is actually a data conversion because 16-bit integer and 32-bit IEEE Float are different underlying
        ///               data types & not directly compatible. the integers must be represented as floats +/- 1.0.
        /// </summary>
        public float[] Buffer16BitIntSamples_to_IEEEFloat32(byte[] input)
        {
            int inputSampleCount = input.Length / 2; // 2 bytes per sample
            float[] output = new float[inputSampleCount];

            int outputIndex = 0;
            for (int a = 0; a < inputSampleCount; a++)
            {
                // convert the next 2 bytes into a 16-bit integer
                short sample_short = BitConverter.ToInt16(input, a * 2);
                // do the arithmetic to convert this integer sample into a 32-bit IEEE Float & store it in the output buffer.
                float sample_float = sample_short / 32768f;
                output[outputIndex++] = sample_float;
            }

            return output;
        }

        /// <summary>
        /// takes a byte[] buffer known to contain 24-bit integer samples & converts to an array of 32-bit IEEE Floating Point samples.
        /// ** IMPORTANT: this is also a data conversion because a conversion to float is required.
        /// </summary>
        public float[] Buffer24BitIntSamples_to_IEEEFloat32(byte[] input)
        {
            int inputSampleCount = input.Length / 3; // 3 bytes per sample
            float[] output = new float[inputSampleCount];

            int outputIndex = 0;
            for (int a = 0; a < inputSampleCount; a++)
            {
                // convert the next 3 bytes into a 24-bit integer.
                int offset = a * 3;
                int sample24 =
                    ((sbyte)input[offset + 2] << 16)
                    | (input[offset + 1] << 8)
                    | input[offset]
                    ;
                // convert this into IEEE Floating Point
                float sample_float = sample24 / 8388608f;
                output[outputIndex++] = sample_float;
            }

            return output;
        }

        #endregion


    }
}

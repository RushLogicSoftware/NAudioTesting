using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.BitDepthConversion
{
    /// <summary>
    /// DEMO CLASS providing some functionality not included with the version of NAudio discussed in the course.
    /// 
    /// allows you to turn a SampleProvider into a 24-bit PCM wave provider (which you could probably then use to do
    /// something like write to a .WAV).
    /// 
    /// ** keep in mind that SampleProviders usually give us 32-bit IEEE Floating Point samples (useful for DSP),
    ///    so we would need to be able to convert this to 24-bit PCM for the .WAV.
    /// </summary>
    public class IEEEFloatToWaveProvider24 : IWaveProvider
    {
        readonly ISampleProvider source; // 32-bit IEEE Floating Point

        public WaveFormat WaveFormat { get { return this.waveFormat; } }
        readonly WaveFormat waveFormat;

        public IEEEFloatToWaveProvider24(ISampleProvider source)
        {
            this.source = source;

            // create a WaveFormat w/ the same sample rate and channel count as the source,
            // but give it a bit depth of 24-bits.
            this.waveFormat = new WaveFormat(
                source.WaveFormat.SampleRate,
                24,
                source.WaveFormat.Channels
                );
        }

        /// <summary>
        /// fills the caller-provided buffer with the specified number of bytes. the # of bytes must be a multiple
        /// of 3, because each group of 3 bytes represents a single 24-bit PCM sample being inserted into the caller-provided buffer.
        /// 
        /// STRATEGY:
        ///     1.) determine correct # of samples requested based on the # of bytes requested, & create a temporary float[]
        ///         that can contain that number of samples.
        ///     2.) read the specified # of float samples from the DSP source ISampleProvider into the temporary float[];
        ///     3.) for each float sample read, clip it to +/- 1, and convert it to the equivalent 24-bit integer sample
        ///         (which must be stored in a 32-bit C# integer).
        ///     4.) place the first 3 bytes of the 32-bit integer variable (representing 24-bit integer data) into the
        ///         caller-provided buffer. we ignore the 4th unnecessary byte in the C# integer variable.
        /// 
        /// </summary>
        /// <param name="buffer">a buffer provided by the caller that we need to populate with data.</param>
        /// <param name="offset"></param>
        /// <param name="count">number of bytes to read, which must be a multiple of 3 because these are 24-bit samples.</param>
        /// <returns># of bytes read</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            // TODO: improve the code by immediately throwing an exception if count is not a multiple of 3.
            // do this before the arithmetic. BlockAlign will be a multiple of 3, and this is all because of
            // the 24-bit requirement. 3 bytes required.


            // determine # of sampels requested & fill a temporary buffer with those samples.
            int numSamplesRequested = count / 3;
            float[] tempBuffer = new float[numSamplesRequested];
            int samplesRead = source.Read(tempBuffer, 0, numSamplesRequested);


            // loop through all samples read
            for (int a = 0; a < samplesRead; a++)
            {
                // grab the sample in 32-bit IEEE Floating Point format & clip it so it stays within +/- 1.
                float sourceSample = tempBuffer[a];
                if (sourceSample > 1f) sourceSample = 1f;
                else if (sourceSample < -1f) sourceSample = -1f;

                // convert the 32-bit IEEE Floating Point sample into a 24-bit int using arithmetic.
                // remember to avoid integer overflow by multiplying by 1 less than the max value of a 24-bit integer.
                int sample24 = (int)(sourceSample * 8388607);

                // use BitConverter to grab the first 3 bytes of the integer sample (ignoring the 4th unnecessary integer byte).
                // place those 3 bytes into the appropriate indexes in the buffer passed in by the caller.
                byte[] sample24_bytes = BitConverter.GetBytes(sample24); // POTENTIAL OPTIMIZATION: raw bit manipulation is faster than BitConverter.

                // IMPORTANT: IT DOES NOT GENERATE SOUND IF YOU DONT INCREMENT OFFSET IN THIS WAY!!
                // what does not work is buffer[offset], buffer[offset+1], buffer[offset+2]
                // WHY IS THAT?!?! we must increment offset, maybe because it is used by the caller??
                buffer[offset++] = sample24_bytes[0];
                buffer[offset++] = sample24_bytes[1];
                buffer[offset++] = sample24_bytes[2];
            }


            // Read() must always return the number of bytes read. 3 bytes have been read per sample.
            return samplesRead * 3;
        }

    }
}

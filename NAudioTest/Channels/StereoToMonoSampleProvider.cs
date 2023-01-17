using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.Channels
{
    /// <summary>
    /// takes a stereo sample provider and evenly mixes the channels to produce mono samples.
    /// </summary>
    public class StereoToMonoSampleProvider : ISampleProvider
    {
        const int inputChannelCount = 2; // <-- the input has to be stereo

        readonly ISampleProvider source;

        public WaveFormat WaveFormat { get { return waveFormat; } }
        readonly WaveFormat waveFormat;


        public float LeftVolume { get; set; }
        public float RightVolume { get; set; }

        public StereoToMonoSampleProvider(ISampleProvider source)
        {
            // make sure the source has 2 channels
            if (source.WaveFormat.Channels != 2)
                throw new ArgumentException("the source sample provider should have 2 channels. it has " + source.WaveFormat.Channels);

            this.source = source;

            // create a WaveFormat with the same sample rate as the source, but with only 1 channel.
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 1);

            // use both source channels evenly, and reduce them by 50% to prevent an increase in volume or clipping.
            this.LeftVolume = 0.5f;
            this.RightVolume = 0.5f;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            // a specific number of channels has been requested, but keep in mind that the source has
            // multiple channels, so the samples for each channel are all one after another in the data structure.
            // so compute the number of source samples we have to actually read from the source provider.
            int numSourceSamples = count * inputChannelCount;

            // read the required number of source samples from the source into a temp buffer.
            float[] temp = new float[numSourceSamples];
            int actualNumSamplesRead = source.Read(temp, 0, numSourceSamples);

            // do the arithmetic to produce the appropriately mixed mono samples & return them.
            // ** KEEP IN MIND THAT IN ORDER FOR THIS TO WORK, we need to do offset++.
            int actualNumOutputSamples = actualNumSamplesRead * inputChannelCount;
            for (int a = 0; a < actualNumOutputSamples; a++)
            {
                // grab the current sample data for each channel.
                // we only have 2 channels in this case, so we are working with Left and Right channels.
                float channel_1 = temp[(a * inputChannelCount)];
                float channel_2 = temp[(a * inputChannelCount) + 1];
                // store the appropriatley mixed sample into the appropriate place in the buffer (using offset++).
                buffer[offset++] = (channel_1 * LeftVolume) + (channel_2 * RightVolume);
            }

            return actualNumOutputSamples;
        }

    }
}

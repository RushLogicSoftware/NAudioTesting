using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace NAudioTest.Providers
{
    /// <summary>
    /// a component in our signal chain which will receive audio data, modify and return it.
    /// the modification will be clipping. we wont allow any samples to be below -1 or above +1.
    /// we will clip samples into that range.
    /// </summary>
    public class ClippingSampleProvider : ISampleProvider
    {

        // this represents the source of the audio data being fed into this signal chain component.
        ISampleProvider source;

        public ClippingSampleProvider(ISampleProvider source)
        {
            this.source = source;
        }


        // we are modifying the audio, but not changing its format. so the source format is the same as the format of this node.
        public WaveFormat WaveFormat { get { return source.WaveFormat; } }


        // because this is a node in the signal chain, reading from it basically means reading from the source while making some modification.
        public int Read(float[] buffer, int offset, int count)
        {
            // read data from the source
            int bytesReadFromSource = source.Read(buffer, offset, count);

            // iterate through the bytes read and adjust (clip) any samples that landed outside of our range.
            // keep in mind the offset while iterating. we dont always read into the beginning of the buffer.
            // NOTE: maybe he avoids defining too many extra variables in an effort to keep things fast and reduce garbage collection.
            for (int a = 0; a < bytesReadFromSource; a++)
            {
                // clip any sample that lands above or below allowable limits
                if (buffer[a + offset] > 1f)
                    buffer[a + offset] = 1f;
                else if (buffer[a + offset] < -1f)
                    buffer[a + offset] = -1f;
            }

            return bytesReadFromSource;
        }
    }
}

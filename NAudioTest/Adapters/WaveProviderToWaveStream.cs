using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.Adapters
{
    public class WaveProviderToWaveStream : WaveStream
    {
        readonly IWaveProvider source;
        long position;

        public WaveProviderToWaveStream(IWaveProvider source)
        {
            this.source = source;
        }


        #region required property overrides
        // the WaveFormat is a passthrough. simply pass the same WaveFormat from the source.
        public override WaveFormat WaveFormat { get { return source.WaveFormat; } }

        // the Length isnt used. simply return the largest possible value.
        public override long Length { get { return long.MaxValue; } }

        public override long Position
        {
            // the Position probably isnt used either. rather than returning 0, we return the position
            // value, which should represent the total amount of data which has been read out so far.
            // again, that technically isnt strictly required anyway.
            get { return position; }
            
            // nobody should try to call the setter. it is invalid in this scenario.
            set { throw new InvalidOperationException("the position cannot be set on an IWaveProvider"); }
        }
        #endregion


        /// <summary>
        /// Read() is another passthrough. it can simply read the underlying source IWaveProvider.
        /// just remember to increment the position each time a read occurs, ALTHOUGH KEEP IN MIND THAT
        /// THIS IS NOT STRICTLY REQUIRED. ITS A NICE THING TO DO, BUT NOT NECESSARILY NEEDED.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var numBytesRead = source.Read(buffer, offset, count);
            position += numBytesRead;
            return numBytesRead;
        }


    }
}

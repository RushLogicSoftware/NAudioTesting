using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using NAudioTest.Codecs.MFT;

namespace NAudioTest
{
    public static class MediaTypeDetailPrinter
    {
        public static void PrintMediaTypeDetails(MftCapabilityChecker.MediaTypeWrapper mt)
        {
            Console.WriteLine("Media Type: " + mt.MajorType + "/" + mt.SubType);
            Console.WriteLine("    AverageBytesPerSecond: " + mt.AverageBytesPerSecond);
            Console.WriteLine("    BitsPerSample: " + mt.BitsPerSample);
            Console.WriteLine("    ChannelCount: " + mt.ChannelCount);
            Console.WriteLine("    SampleRate: " + mt.SampleRate);
        }
    }
}

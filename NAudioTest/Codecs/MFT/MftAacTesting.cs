using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.MediaFoundation;
using NAudio.Wave;

namespace NAudioTest.Codecs.MFT
{
    public class MftAacTesting
    {

        public void Convert_MP3_to_AAC(
            string inputMp3FilePath, string outputAacFilePath,
            int? preferredBitRate = null
            )
        {
            using (var reader = new MediaFoundationReader(inputMp3FilePath))
            {
                if (preferredBitRate == null)
                    MediaFoundationEncoder.EncodeToAac(reader, outputAacFilePath);
                else
                    MediaFoundationEncoder.EncodeToAac(reader, outputAacFilePath, preferredBitRate.Value);
            }
        }

    }
}

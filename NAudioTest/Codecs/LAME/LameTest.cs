using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace NAudioTest.Codecs.LAME
{
    /// <summary>
    /// THIS DOES NOT ACTUALLY WORK AS ADVERTISED.
    /// unless i simply dont have the command line options required, this is doing nothing more than renaming the
    /// file, maybe while modifying the bitrate. it does not change .WAV to .MP3 or vice versa. in order to get it
    /// to actually change formats, additional research & command line options may be required.
    /// </summary>
    public class LameTest
    {
        public void LameConversion(string sourceFilePath, string destinationFilePath, int bitRate_kbps)
        {
            Process proc = Process.Start(new ProcessStartInfo {
                FileName = LameConfiguration.LameExePath,
                Arguments = "-b " + bitRate_kbps + " \"" + sourceFilePath + "\" \"" + destinationFilePath + "\"",
                WindowStyle = ProcessWindowStyle.Normal
            });
            proc.WaitForExit();
        }
    }
}


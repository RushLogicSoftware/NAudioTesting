using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using NAudioTest.EditingAudio;

namespace NAudioTest._TestOptionSelection
{
    public static class AudioExtractionTestOptions
    {
        public static void TestAudioExtraction()
        {
            string trimmedFilePath = null;
            Console.WriteLine("what type of audio extraction would you like to test?");
            Console.WriteLine("    MP3");
            switch (Console.ReadLine().ToLower())
            {
                case "mp3":
                    MP3Extractor extractor = new MP3Extractor();
                    trimmedFilePath = extractor.ExtractMP3AudioToSeparateFile();
                    break;
            }
            Console.WriteLine("extracted a portion of the audio into file: " + new FileInfo(trimmedFilePath).Name);
            Console.WriteLine("press <ENTER> to continue.");
            Console.ReadLine();
        }
    }
}

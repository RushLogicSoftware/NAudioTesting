using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest._TestOptionSelection
{
    public static class AudioFileDetailTestOptions
    {
        public static void TestAudioFileDetailPrinting()
        {
            Console.Clear();
            Console.WriteLine("what file format do you want to see details for?");
            Console.WriteLine("    WAV");
            Console.WriteLine("    MP3");
            string inputFilePath = null;
            switch (Console.ReadLine().ToLower())
            {
                case "wav":
                    inputFilePath = AudioTestingPaths.VoiceQuote_1;
                    break;
                case "mp3":
                    inputFilePath = AudioTestingPaths.MediumAudioFile;
                    break;
            }
            AudioFileDetailPrinter.PrintFileDetails(inputFilePath);
            Console.WriteLine("press <ENTER> to continue.");
            Console.ReadLine();
        }
    }
}

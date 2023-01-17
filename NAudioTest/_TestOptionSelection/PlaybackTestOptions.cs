using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest._TestOptionSelection
{
    public static class PlaybackTestOptions
    {
        public static void TestPlayback()
        {
            Console.WriteLine("testing Playback. what type of playback would you like to test?");
            Console.WriteLine("type one of the following options, then hit enter:");
            Console.WriteLine("    WaveOut");
            switch (Console.ReadLine().ToLower())
            {
                case "waveout":
                    WaveOutTestOptions.TestWaveOut();
                    break;
            }
        }
    }
}

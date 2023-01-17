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
    public static class AudioConcatenationTestOptions
    {
        public static void TestAudioFileConcatenation()
        {
            AudioConcatinator concatinator = new AudioConcatinator();
            concatinator.Concatinate(
                new List<string> {
                    AudioTestingPaths.VoiceQuote_1,
                    AudioTestingPaths.VoiceQuote_3,
                    AudioTestingPaths.VoiceQuote_2
                },
                AudioTestingPaths.NAudioTestingDirPath + "Concatenation.wav"
                );
        }
    }
}

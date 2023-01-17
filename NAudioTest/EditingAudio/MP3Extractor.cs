using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;

namespace NAudioTest.EditingAudio
{
    public class MP3Extractor
    {
        public string ExtractMP3AudioToSeparateFile()
        {
            DirectoryInfo NAudioTestingDir = new DirectoryInfo(AudioTestingPaths.NAudioTestingDirPath);
            if (!NAudioTestingDir.Exists) NAudioTestingDir.Create();

            string extension = new FileInfo(AudioTestingPaths.LongAudioFile).Extension;
            string trimmedFilePath = AudioTestingPaths.LongAudioFile.Substring(0, AudioTestingPaths.LongAudioFile.Length - extension.Length) +
                "_Trimmed" + extension
                ;

            // open a reader to read the source data, and a writer to output the new, shorter file.
            using (var mp3Reader = new Mp3FileReader(AudioTestingPaths.LongAudioFile))
            using (var writer = File.Create(trimmedFilePath))
            {
                TimeSpan startTime = new TimeSpan(0, 12, 45);
                TimeSpan endTime = new TimeSpan(0, 13, 40);
                // set the current time as the start time.
                mp3Reader.CurrentTime = startTime;
                // read each frame from the current time until we hit a frame that is after our end time.
                // THIS IS FAR FROM PERFECT because the beginning/end of the frames wont align perfectly
                // with the start/end time we set. it will just be a loose approximation.
                while (mp3Reader.CurrentTime < endTime)
                {
                    // read the next frame, which automatically advances CurrentTime
                    var nextFrame = mp3Reader.ReadNextFrame();

                    // break the loop if there are no more frames.
                    if (nextFrame == null) break;

                    // write all of the data from this frame into the file
                    writer.Write(nextFrame.RawData, 0, nextFrame.RawData.Length);
                }
            }

            return trimmedFilePath;
        }
    }
}

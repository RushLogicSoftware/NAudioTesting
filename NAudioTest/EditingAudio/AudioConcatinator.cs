using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;

namespace NAudioTest.EditingAudio
{
    public class AudioConcatinator
    {

        /// <summary>
        /// concatenates audio files of the same format into one big audio file.
        /// 
        /// POTENTIAL IMPROVEMENT: start with a verification stage which will open a WaveFileReader for
        ///                        each file to gather information, including its WaveFormat. throw an exception
        ///                        right away or return some status indicating that the files are not all in the
        ///                        same format. do this before beginning the concatination.
        /// </summary>
        /// <param name="sourceFilePaths">source audio files, all in the same format.</param>
        /// <param name="outputFilePath">output audio file path</param>
        public void Concatinate(List<string> sourceFilePaths, string outputFilePath)
        {
            // create a large buffer
            byte[] buffer = new byte[44100 * 2];

            // when reading the first file, we will discover the wave format & create an appropriate writer.
            WaveFormat waveFormat = null;
            WaveFileWriter waveWriter = null;

            sourceFilePaths.ForEach(sourcePath => {
                using (var reader = new WaveFileReader(sourcePath))
                {
                    bool includeFileInConcatination = true;

                    // if this is the first file being read, discover the format & create the writer.
                    // otherwise, make sure the format of this file matches the format we are already writing.
                    if (waveFormat == null)
                    {
                        waveFormat = reader.WaveFormat;
                        waveWriter = new WaveFileWriter(outputFilePath, waveFormat);
                    }
                    else if (!reader.WaveFormat.Equals(waveFormat))
                    {
                        // this file cannot be included because it doesnt have the correct format.
                        includeFileInConcatination = false;
                    }

                    // if this file is to be included, add its audio data to the file.
                    if (includeFileInConcatination)
                    {
                        int bytesRead;
                        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                            waveWriter.Write(buffer, 0, bytesRead);
                    }
                }
            });

            // finally call dispose, because otherwise the file wont be playable
            waveWriter.Dispose();
        }
    }
}

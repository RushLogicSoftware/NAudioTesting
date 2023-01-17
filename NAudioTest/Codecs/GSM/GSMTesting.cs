using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using NAudioTest.ReadingFormatDetails;

namespace NAudioTest.Codecs.GSM
{
    public class GSMTesting
    {

        /// <summary>
        /// uses WaveFormatConversionStream, which is an easy way to access ACM codecs for audio conversion.
        /// takes the input .WAV file data and passes it through WaveFormatConversionStream, which will detect
        /// the appropriate Codec to use for the conversion based on the specified WaveFormat.
        /// </summary>
        public void Convert_PCM_to_GSM610(
            string inputWaveFilePath_PCM_8kHz_16bit_Mono, string outputWaveFilePath
            )
        {


            // TODO: apply knowledge from earlier lessons.
            // if the input file is not in the correct format, take necessary steps to convert the format:
            //  ** resample to 8kHz
            //  ** change bit depth to 16 bits per sample
            //  ** go from stereo to mono


            #region first verify that the input audio file is the correct expected format.
            AudioFormatDetailReader detailReader = new AudioFormatDetailReader();
            AudioFormatDetailReader.AudioFileFormatDetails details = detailReader.GetAudioFileFormatDetails(inputWaveFilePath_PCM_8kHz_16bit_Mono);
            if(details.Encoding != WaveFormatEncoding.Pcm)
                throw new ArgumentException("the input .WAV file must be in PCM encoding. The encoding is: " + details.Encoding);
            if (details.SampleRate != 8000)
                throw new ArgumentException("the input .WAV file must have a sample rate of 8kHz. The sample rate is: " + details.SampleRate);
            if (details.BitsPerSample != 16)
                throw new ArgumentException("the input .WAV file must have a bit depth of 16-bit. The bit depth is: " + details.BitsPerSample);
            if (details.ChannelCount != 1)
                throw new ArgumentException("the input .WAV file must be mono (1 channel). The number of channels is: " + details.ChannelCount);
            #endregion

            // instantiate a special WaveFormat, which is designed to exactly match the formatting
            // expected by the GSM 6.10 codec.
            WaveFormat targetFormat = new Gsm610WaveFormat();

            using (var reader = new WaveFileReader(inputWaveFilePath_PCM_8kHz_16bit_Mono))
            using (var converter = new WaveFormatConversionStream(targetFormat, reader))
            {
                WaveFileWriter.CreateWaveFile(outputWaveFilePath, converter);
            }
        }



        /// <summary>
        /// performs the reverse conversion.
        /// </summary>
        public void Convert_GSM610_to_PCM(
            string inputWaveFilePath_GSM610, string outputWaveFilePath
            )
        {

            // TODO: apply knowledge from earlier lessons.
            // if the input file is not in the correct format, take necessary steps to convert the format


            #region first verify that the input audio file is the correct expected format.
            AudioFormatDetailReader detailReader = new AudioFormatDetailReader();
            AudioFormatDetailReader.AudioFileFormatDetails details = detailReader.GetAudioFileFormatDetails(inputWaveFilePath_GSM610);
            if (details.Encoding != WaveFormatEncoding.Gsm610)
                throw new ArgumentException("the input .WAV file must be in GSM 6.10 encoding. The encoding is: " + details.Encoding);
            //if (details.SampleRate != 8000)
            //    throw new ArgumentException("the input .WAV file must have a sample rate of 8kHz. The sample rate is: " + details.SampleRate);
            //if (details.BitsPerSample != 16)
            //    throw new ArgumentException("the input .WAV file must have a bit depth of 16-bit. The bit depth is: " + details.BitsPerSample);
            //if (details.ChannelCount != 1)
            //    throw new ArgumentException("the input .WAV file must be mono (1 channel). The number of channels is: " + details.ChannelCount);
            #endregion



            using (var reader = new WaveFileReader(inputWaveFilePath_GSM610))
            {
                // OPTION 1: for the conversion stream, tell it the output WaveFormat which is appropriate for
                // storing voice audio that came from GSM. we use 8kHz 16-bit mono, which is the same as the input for GSM 6.10 encoding.
                using (var converter = new WaveFormatConversionStream(new WaveFormat(8000, 16, 1), reader))

                // OPTION 2: it is easier to just use CreatePcmStream(), which figures out how to setup the WaveFormatConversionStream.
                // THE PROBLEM IS, this is an extension method, and Mark Heath never tells us where to find extension methods or
                // what using directives to use for any of the exercises... thanks...
                //using (var converter = new WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(outputWaveFilePath, converter);
                }
            }

        }





    }
}

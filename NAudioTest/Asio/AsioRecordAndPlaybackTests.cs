using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace NAudioTest.Asio
{
    /// <summary>
    /// playback and record is done together in Asio, because it has a flow of data that
    /// handles capturing samples and playing samples as a single high-performance pipeline.
    /// 
    /// CRITICALLY IMPORTANT!!!!!!!!!
    /// CRITICALLY IMPORTANT!!!!!!!!!
    ///     [STAThread] attribute must be applied on the application. put this on the Main method if this
    ///     is a console application, and do research to determine how to activate it in other ways, like
    ///     with Windows Forms. this allows the AsioOut class to communicate via COM, so this code cannot
    ///     work otherwise!!!!!!!!!!!
    /// 
    /// </summary>
    public class AsioRecordAndPlaybackTests
    {
        public AsioConfigOptions GetConfigOptions()
        {
            List<AsioDriver> Drivers = new List<AsioDriver>();
            foreach (string driverName in AsioOut.GetDriverNames())
            {
                // populate a dictionary of basic input channel numbers and names.
                Dictionary<int, string> inputChannels = new Dictionary<int, string>();
                using (var asioDriver = new AsioOut(driverName))
                {
                    int DriverInputChannelCount = asioDriver.DriverInputChannelCount;
                    for (int a = 0; a < DriverInputChannelCount; a++)
                    {
                        int channelNum = a + 1;
                        inputChannels.Add(channelNum, "Asio Input Channel " + channelNum);
                    }
                }

                Drivers.Add(new AsioDriver {
                    Name = driverName,
                    InputChannels = inputChannels
                });
            }

            return new AsioConfigOptions {
                Drivers = Drivers
            };
        }


        public AsioConfiguration asioConfiguration { get; set; }
        AsioOut asio;
        public void ConfigureAsio(AsioConfiguration asioConfiguration)
        {
            this.asioConfiguration = asioConfiguration;

            // validate the selected input channel configuration
            if (asioConfiguration.inputChannelSelection == eInputChannelSelection.SpecificRange)
            {
                if (asioConfiguration.ChannelRange_FirstChannel == null || asioConfiguration.ChannelRange_LastChannel == null)
                    throw new Exception(
                        "an attempt has been made to configure a specific range of Asio input channels,\r\n" +
                        "but the first and last channels of that range have not been configured."
                        );
                if (asioConfiguration.ChannelRange_FirstChannel.Value > asioConfiguration.ChannelRange_LastChannel.Value)
                    throw new Exception(
                        "invalid channel range. the first channel cannot be less than the last channel."
                        );
                HashSet<int> validChannelNumbers = new HashSet<int>(asioConfiguration.SelectedDriver.InputChannels.Keys);
                if (!validChannelNumbers.Contains(asioConfiguration.ChannelRange_FirstChannel.Value) || !validChannelNumbers.Contains(asioConfiguration.ChannelRange_LastChannel.Value))
                    throw new Exception(
                        "the starting and ending channels in the range must be valid channel numbers. see the selected AsioDriver for valid channel numbers."
                        );
            }

            recordingChannelCount = asioConfiguration.inputChannelSelection == eInputChannelSelection.AllChannels ?
                asioConfiguration.SelectedDriver.InputChannels.Count
                :
                asioConfiguration.ChannelRange_LastChannel.Value - asioConfiguration.ChannelRange_FirstChannel.Value + 1
                ;

            // construct the appropriate WaveFormat depending on 
            recordingFormat = WaveFormatConfig.GetWaveFormat(
                // asio samples at the precise configured rate, and shares the samples across channels.
                // so each channel only gets its fraction of those samples.
                WaveFormatConfig.SampleRates[asioConfiguration.recordingSampleRate] / recordingChannelCount,
                asioConfiguration.recordingBitDepth,

                // for proper functionality, set recording channels
                recordingChannelCount

                );


            this.asio = new AsioOut(asioConfiguration.SelectedDriver.Name);
        }

        WaveFormat recordingFormat;

        int recordingChannelCount;
        //WaveFileWriter writer;
        Dictionary<int, WaveFileWriter> ChannelWriters;
        Dictionary<int, float[]> ChannelBuffers;

        public void StartDataFlow(string recordingFilePath)
        {
            // configure recording.
            // by default, set it to record on all channels
            int recordingChannelCount = asioConfiguration.SelectedDriver.InputChannels.Count;
            // if a specific range of channels was selected, configure that range
            if (asioConfiguration.inputChannelSelection == eInputChannelSelection.SpecificRange)
            {
                asio.InputChannelOffset = asioConfiguration.ChannelRange_FirstChannel.Value - 1;
                recordingChannelCount = asioConfiguration.ChannelRange_LastChannel.Value - asioConfiguration.ChannelRange_FirstChannel.Value + 1;
            }

            // instantiate recording channels
            //writer = new WaveFileWriter(recordingFilePath, recordingFormat);
            ChannelWriters = new Dictionary<int, WaveFileWriter>();
            ChannelBuffers = new Dictionary<int, float[]>();
            for (int a = 0; a < recordingChannelCount; a++)
            {
                int channelNum = a + 1;
                ChannelWriters.Add(channelNum, new WaveFileWriter(recordingFilePath.Replace(".wav", "_Channel" + channelNum + ".wav"), recordingFormat));
                ChannelBuffers.Add(channelNum, null);
            }

            asio.AudioAvailable += Asio_AudioAvailable;
            asio.PlaybackStopped += Asio_PlaybackStopped;

            int recordingSampleRate = WaveFormatConfig.SampleRates[asioConfiguration.recordingSampleRate];
            asio.InitRecordAndPlayback(
                null, // <-- null IWaveProvider means there is nothing to play, so Asio will play nothing.
                recordingChannelCount,
                recordingSampleRate
                );

            // start the flow of data.
            asio.Play();
        }


        private void Asio_AudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            // get the interleaved samples
            float[] interleavedSamples = e.GetAsInterleavedSamples();

            // break the samples into separate, appropriately sized buffers (one for each channel)
            int samplesPerChannel = interleavedSamples.Length / recordingChannelCount;
            for (int a = 0; a < recordingChannelCount; a++)
                ChannelBuffers[a + 1] = new float[samplesPerChannel];

            // populate the buffers with their sample data
            int channelBufferIndex = 0;
            for (int a = 0; a < interleavedSamples.Length; a += recordingChannelCount)
            {
                for (int channelIdx = 0; channelIdx < recordingChannelCount; channelIdx++)
                    ChannelBuffers[channelIdx + 1][channelBufferIndex] = interleavedSamples[a + channelIdx];
                channelBufferIndex++;
            }

            // write the buffered data into each channel
            for (int channelNum = 1; channelNum <= recordingChannelCount; channelNum++)
                ChannelWriters[channelNum].WriteSamples(ChannelBuffers[channelNum], 0, samplesPerChannel);

            //writer.WriteSamples(interleavedSamples, 0, interleavedSamples.Length);
        }


        public void StopDataFlow()
        {
            asio.Stop();
        }

        private void Asio_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
                throw new Exception("an unknown error has caused the Asio audio data flow to stop. see inner exception.", e.Exception);

            // dispose the writers
            for (int channelNum = 1; channelNum <= recordingChannelCount; channelNum++)
            {
                ChannelWriters[channelNum].Dispose();
                ChannelWriters[channelNum] = null;
            }
            //writer.Dispose();

            asio.Dispose();
        }

    }
}

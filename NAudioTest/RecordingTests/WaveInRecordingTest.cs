using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;


// some WasapiCapture functionality is in here apparently.
using NAudio.CoreAudioApi;


namespace NAudioTest.RecordingTests
{
    public class WaveInRecordingTest
    {
        public WaveInConfigOptions GetConfigOptions()
        {
            WaveInConfigOptions options = new WaveInConfigOptions {
                Devices = new List<InputDevice>()
            };

            Func<string, string> sanitize = (str) => { return new string(str.ToUpper().Where(c => char.IsLetterOrDigit(c)).ToArray()); };

            // populate list of available audio devices
            for (int a = 0; a < WaveIn.DeviceCount; a++)
            {
                WaveInCapabilities deviceCapabilities = WaveIn.GetCapabilities(a);
                options.Devices.Add(new InputDevice {
                    Name = deviceCapabilities.ProductName,
                    DeviceIndex = a,
                    ChannelCount = deviceCapabilities.Channels
                });
            }

            // see if we can improve any truncated device names by loading their full names using WasapiCapture MMDeviceEnumerator
            List<InputDevice> truncatedDevices = options.Devices.Where(device => device.Name.Length >= 30).ToList();
            MMDevice[] CaptureDevices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            CaptureDevices.ToList().ForEach(device => {
                List<InputDevice> matchingDevices = truncatedDevices
                    .Where(existingDevice => {
                        string sanitizedExistingDevice = sanitize(existingDevice.Name);
                        string sanitizedDirectSoundDevice = sanitize(device.FriendlyName);
                        return sanitizedDirectSoundDevice.StartsWith(sanitizedExistingDevice);
                    })
                    .ToList()
                    ;
                if (matchingDevices.Count == 1)
                {
                    // we have a match
                    matchingDevices.Single().Name = device.FriendlyName;
                }
            });


            return options;
        }

        public WaveInRecordingConfiguration recordingConfig { get; set; }
        public void ConfigureAudioRecording(WaveInRecordingConfiguration recordingConfig)
        {
            this.recordingConfig = recordingConfig;

            // construct the appropriate WaveFormat depending on 
            recordingFormat = WaveFormatConfig.GetWaveFormat(
                recordingConfig.sampleRate,
                recordingConfig.bitDepth,
                recordingConfig.selectedDevice.ChannelCount
                );
        }

        WaveFormat recordingFormat;


        enum eRecordingState { Stopped, Monitoring, Recording, RequestedStop };
        eRecordingState recordingState;


        WaveInEvent wiEvent;

        WaveFileWriter writer;


        public void BeginReceivingAudio()
        {
            // make sure this is a valid call.
            if (recordingConfig == null || recordingConfig.selectedDevice == null)
                throw new InvalidOperationException(
                    "before audio data can be received, a WaveInRecordingConfiguration must be provided, " +
                    "and an audio recording device should have been selected."
                    );
            if (recordingState != eRecordingState.Stopped)
                throw new InvalidOperationException("Audio data cannot be received while we are in the " + recordingState.ToString() + " state.");


            int recordingDevice = recordingConfig.selectedDevice.DeviceIndex;

            #region PROBLEMATIC EXAMPLE CODE
            // althrough the following code came from Mark Heaths demo application AudioRecorder.cs > BeginMonitoring();
            // it doesnt work. the WaveIn constructor seems to be forcing us to make certain architectural software
            // decisions by telling us what our threading has to do. simply calling the constructor (regardless of whether it
            // is on a background thread) triggers the following exception:
            //      "Use WaveInEvent to record on a background thread"

            // WaveInEvent is similar to WaveIn but does not inherit from it. but apparently WaveInEvent has to be used instead.
            // the explanations are unclear.
            //waveIn = new WaveIn();
            //waveIn.DeviceNumber = recordingDevice;
            //waveIn.DataAvailable += OnDataAvailable;
            //waveIn.RecordingStopped += OnRecordingStopped;
            //waveIn.WaveFormat = recordingFormat;
            //waveIn.StartRecording();
            #endregion

            
            #region using WaveInEvent as a workaround for WaveIn background thread issue.
            wiEvent = new WaveInEvent();
            wiEvent.DeviceNumber = recordingDevice;
            wiEvent.DataAvailable += OnDataAvailable;
            wiEvent.RecordingStopped += OnRecordingStopped;
            wiEvent.WaveFormat = recordingFormat;
            wiEvent.StartRecording();
            #endregion

            // volume control not yet implemented.
            //TryGetVolumeControl();

            recordingState = eRecordingState.Monitoring;
        }


        string waveFileName;
        public void BeginRecording(string waveFileName)
        {
            this.waveFileName = waveFileName;

            if (recordingState != eRecordingState.Monitoring)
                throw new InvalidOperationException("unable to begin recording when data is not currently being received. current state is: " + recordingState.ToString());

            // the original single-channel way.
            writer = new WaveFileWriter(waveFileName, recordingFormat);

            // custom enhanced technique for splitting channels
            //ConfigureChannelOutputs(waveFileName);

            recordingState = eRecordingState.Recording;
        }



        float highestSample = 0;


        /// <summary>
        /// IMPORTANT: if the input device has multiple channels, the sample data is interleaved. this is probably
        ///            not accounted for in WriteToFile(), so we are writing only one channel of data.
        /// </summary>
        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            long maxFileLength = this.recordingFormat.AverageBytesPerSecond * 60;

            // if we are in recording mode, write the raw PCM sample bytes to a file.
            if (recordingState == eRecordingState.Recording || recordingState == eRecordingState.RequestedStop)
            {
                // Marks default implementation for writing to a single output.
                WriteToFile(buffer, bytesRecorded, maxFileLength);
                // custom implementation for writing multiple outputs.
                //WriteToChannelFiles(buffer, bytesRecorded);
            }

            // interpret the bytes as 32-bit floating point samples.
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;

                // TODO: ADD THE SAMPLE TO SOME COLLECTION TO DRIVE MONITORING LATER.
                //sampleAggregator.Add(sample32);
                // keep track of the highest incoming sample.
                if (sample32 > highestSample)
                    highestSample = sample32;
            }
        }

        private void WriteToFile(byte[] buffer, int bytesRecorded, long maxFileLength)
        {
            var toWrite = (int)Math.Min(maxFileLength - writer.Length, bytesRecorded);
            if (toWrite > 0)
            {
                writer.Write(buffer, 0, bytesRecorded);
            }
            else
            {
                Stop();
            }
        }


        public void Stop()
        {
            if (recordingState == eRecordingState.Recording)
            {
                recordingState = eRecordingState.RequestedStop;

                // cant use WaveIn apparently...
                //waveIn.StopRecording();

                wiEvent.StopRecording();
            }
        }


        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            recordingState = eRecordingState.Stopped;
            writer.Dispose();
        }


        #region CHANNEL SPLITTING EXPERIMENTS

        #region CHANNEL SPLITTING FROM FILE
        public static void SplitChannels(string inputWav, string outputWav)
        {
            var reader = new WaveFileReader(inputWav);
            var writers = new WaveFileWriter[reader.WaveFormat.Channels];
            for (int n = 0; n < writers.Length; n++)
            {
                var format = new WaveFormat(reader.WaveFormat.SampleRate, 16, 1);
                writers[n] = new WaveFileWriter(
                    outputWav.Replace(".wav", "_Channel_" + (n + 1) + ".wav"),
                    format
                    );
            }

            float[] buffer;
            while ((buffer = reader.ReadNextSampleFrame())?.Length > 0)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    // write one sample for each channel (i is the channelNumber)
                    writers[i].WriteSample(buffer[i]);
                }
            }

            for (int n = 0; n < writers.Length; n++)
            {
                writers[n].Dispose();
            }
            reader.Dispose();

            #region EXAMPLE STRATEGY FROM COURSE DOES NOT WORK
            /*
            var reader = new WaveFileReader(inputWav);
            var buffer = new byte[2 * reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
            var writers = new WaveFileWriter[reader.WaveFormat.Channels];
            for (int n = 0; n < writers.Length; n++)
            {
                var format = new WaveFormat(reader.WaveFormat.SampleRate, 16, 1);
                writers[n] = new WaveFileWriter(
                    outputWav.Replace(".wav", "_Channel_" + (n + 1) + ".wav"),
                    format
                    );
            }
            int bytesRead;
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                int offset = 0;
                while (offset < bytesRead)
                {
                    for (int n = 0; n < writers.Length; n++)
                    {
                        // write one sample
                        writers[n].Write(buffer, offset, 2);
                        offset += 2;
                    }
                }
            }
            for (int n = 0; n < writers.Length; n++)
            {
                writers[n].Dispose();
            }
            reader.Dispose();
            */
            #endregion
        }
        #endregion


        byte[] channelOutputBuffer;

        class ChannelOutput
        {
            public WaveFileWriter writer { get; set; }
        }
        Dictionary<int, ChannelOutput> ChannelOutputs;
        void ConfigureChannelOutputs(string waveFileName)
        {
            ChannelOutputs = new Dictionary<int, ChannelOutput>();
            for (int a = 0; a < recordingConfig.selectedDevice.ChannelCount; a++)
            {
                int channelNum = a + 1;
                string outputFilePath = waveFileName.Replace(".wav", "_Channel" + channelNum + ".wav");
                ChannelOutputs.Add(
                    channelNum,
                    new ChannelOutput {
                        writer = new WaveFileWriter(outputFilePath, recordingFormat)
                    }
                    );
            }

            // configure the channel output buffer which will be used to write to output channel files.
            int bytesPerSample = 2; // 16-bit PCM
            channelOutputBuffer = new byte[bytesPerSample * recordingFormat.SampleRate * recordingFormat.Channels];
        }

        private void WriteToChannelFiles(byte[] buffer, int bytesRecorded)
        {
            if (bytesRecorded > 0)
            {
                //channelOutputBuffer


                //for (int a = 0; a < recordingConfig.selectedDevice.ChannelCount; a++)
                //{
                //    int channelNum = a + 1;
                //    ChannelOutputs.Add(
                //        channelNum,
                //        new ChannelOutput
                //        {
                //            writer = new WaveFileWriter(outputFilePath, recordingFormat)
                //        }
                //        );
                //}
            }
        }

        #endregion
    }
}

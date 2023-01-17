using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using NAudio.MediaFoundation;
using NAudio.Wave;

using NAudioTest.PlaybackTests;
using NAudioTest.EditingAudio;
using NAudioTest.FormatConversions;
using NAudioTest.SynthesizingSamples;
using NAudioTest.ReadingFormatDetails;
using NAudioTest.Resampling;
using NAudioTest.BitDepthConversion;
using NAudioTest.Channels;
using NAudioTest.Codecs.GSM;
using NAudioTest.Codecs.MFT;
using NAudioTest.Codecs.LAME;
using NAudioTest.Mixing;
using NAudioTest.RecordingTests;
using NAudioTest.Asio;

namespace NAudioTest._TestOptionSelection
{
    public static class RecordingTestOptions
    {
        public static void TestRecording()
        {
            Console.WriteLine("testing Recording. what type of playback would you like to test?");
            Console.WriteLine("    WaveIn");
            Console.WriteLine("    Asio");
            switch (Console.ReadLine().ToLower())
            {
                case "wavein":
                    TestWaveIn();
                    break;
                case "asio":
                    TestAsio();
                    break;
            }
        }



        static void TestWaveIn()
        {
            WaveInRecordingTest recTest = new WaveInRecordingTest();
            bool testingRecording = true;

            #region configure the recorder
            WaveInConfigOptions waveInConfig = recTest.GetConfigOptions();
            Console.WriteLine("the following audio input devices are available");
            Dictionary<int, InputDevice> devices = new Dictionary<int, InputDevice>();
            int printNum = 1;
            waveInConfig.Devices.ForEach(device => {
                devices.Add(printNum, device);
                Console.WriteLine("    " + printNum + ".) " + device.Name);
                printNum++;
            });
            Console.WriteLine("enter the number corresponding to the audio input device you want to use.");
            InputDevice selectedDevice = devices[Int32.Parse(Console.ReadLine())];

            List<int> selectedChannels = new List<int>();
            if (selectedDevice.ChannelCount > 1)
            {
                Console.WriteLine("the selected device has " + selectedDevice.ChannelCount + " input channels.");
                Console.WriteLine("specify the recording channels by entering a comma-separated list of channel numbers, which can be from 1 to " + selectedDevice.ChannelCount);
                selectedChannels.AddRange(
                    Console.ReadLine().Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Int32.Parse(s))
                    );
            }


            eSampleRate selectedSamplingRate = GetUserSelectedSampleRate();

            eBitDepth selectedBitDepth = GetUserSelectedBitDepth();

            // set some initial configurations
            recTest.ConfigureAudioRecording(new WaveInRecordingConfiguration {
                selectedDevice = selectedDevice,

                SelectedChannels = selectedChannels,
                sampleRate = selectedSamplingRate,
                bitDepth = selectedBitDepth,

                //VolumeControlType = volumeControlType,
                // for now we are not configuring these additional settings. come back and implement if necessary.
                NumberOfBuffers = null,
                DesiredLatency = null,
                InitialVolume = null
            });


            #endregion


            testingRecording = true;
            while (testingRecording)
            {
                Console.Clear();
                Console.WriteLine("enter the action you want to perform:");
                Console.WriteLine("    options are:");
                Console.WriteLine("         record");
                Console.WriteLine("         stop");
                switch (Console.ReadLine().ToLower())
                {
                    case "record":
                        // begin receiving audio data from the selected recording device.
                        recTest.BeginReceivingAudio();
                        // begin recording to a file
                        recTest.BeginRecording(AudioTestingPaths.RawAudioRecordingOutputPath);
                        break;
                    case "stop":
                        recTest.Stop();
                        testingRecording = false;
                        break;
                }
            }
        }


        static void TestAsio()
        {
            AsioRecordAndPlaybackTests asioTests = new AsioRecordAndPlaybackTests();

            // get asio config options
            AsioConfigOptions asioOptions = asioTests.GetConfigOptions();

            // select an available driver
            string selectedDriverName = UserOptionSelector.GetUserSelectedOption(
                // available driver names
                asioOptions.Drivers.Select(driver => driver.Name).ToList(),
                "available Asio drivers",
                "enter the number corresponding to the Asio driver you want to select"
                );
            AsioDriver selectedDriver = asioOptions.Drivers.Single(driver => driver.Name == selectedDriverName);

            // configure channels
            List<string> channelNames = selectedDriver.InputChannels.Values.ToList();
            string firstChannelName = UserOptionSelector.GetUserSelectedOption(channelNames, "available input channels", "enter the first input channel you want to use.");
            string lastChannelName = UserOptionSelector.GetUserSelectedOption(channelNames, "available input channels", "enter the last input channel you want to use.");
            int firstChannelNum = selectedDriver.InputChannels.Single(kvp => kvp.Value == firstChannelName).Key;
            int lastChannelNum = selectedDriver.InputChannels.Single(kvp => kvp.Value == lastChannelName).Key;


            Console.WriteLine();
            Console.WriteLine("==== IMPORT SAMPLING RATE OBSERVATION ====");
            Console.WriteLine("Asio does not appear to support low sample rates, and it throws exceptions.");
            Console.WriteLine("use at least 44.1kHz");
            Console.WriteLine();
            eSampleRate recordingSampleRate = GetUserSelectedSampleRate();
            eBitDepth recordingBitDepth = GetUserSelectedBitDepth();

            // configure asio
            asioTests.ConfigureAsio(new AsioConfiguration {
                SelectedDriver = selectedDriver,
                recordingSampleRate = recordingSampleRate,
                recordingBitDepth = recordingBitDepth,
                inputChannelSelection = eInputChannelSelection.SpecificRange,
                ChannelRange_FirstChannel = firstChannelNum,
                ChannelRange_LastChannel = lastChannelNum
            });

            // start the flow of data. we will record audio.
            asioTests.StartDataFlow(AudioTestingPaths.AsioRecordingFilePath);

            // after a delay, stop the flow of data.
            Task.Factory.StartNew(() => {
                // wait for 10 seconds
                Thread.Sleep(20000);
                // stop the flow
                asioTests.StopDataFlow();
            });
        }


        static eSampleRate GetUserSelectedSampleRate()
        {
            List<string> printOptions = Enum.GetValues(typeof(eSampleRate)).OfType<eSampleRate>().Select(x => x.ToString()).ToList();
            string printLabel = "available sampling rates are:";
            string instructions = "enter the number corresponding to the sampling rate you would like to use for recording:";
            string selectedOption = UserOptionSelector.GetUserSelectedOption(printOptions, printLabel, instructions);
            eSampleRate selectedSamplingRate = (eSampleRate)Enum.Parse(typeof(eSampleRate), selectedOption);
            return selectedSamplingRate;
        }
        static eBitDepth GetUserSelectedBitDepth()
        {
            List<string> printOptions = Enum.GetValues(typeof(eBitDepth)).OfType<eBitDepth>().Select(x => x.ToString()).ToList();
            string printLabel = "available bit depths are:";
            string instructions = "enter the number corresponding to the bit depth you would like to use for recording:";
            string selectedOption = UserOptionSelector.GetUserSelectedOption(printOptions, printLabel, instructions);
            eBitDepth selectedBitDepth = (eBitDepth)Enum.Parse(typeof(eBitDepth), selectedOption);
            return selectedBitDepth;
        }

    }
}

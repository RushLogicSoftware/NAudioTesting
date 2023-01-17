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
    public static class WaveOutTestOptions
    {
        public static void TestWaveOut()
        {
            WaveOutPlaybackTest playback = new WaveOutPlaybackTest();
            bool testingPlayback = true;
            TimeSpan? AudioFileDuration = null;

            #region configure the player
            WaveOutConfigOptions waveOutConfig = playback.GetConfigOptions();
            Console.WriteLine("the following audio devices are available");
            Dictionary<int, AudioDevice> devices = new Dictionary<int, AudioDevice>();
            int printNum = 1;
            waveOutConfig.Devices.ForEach(device => {
                devices.Add(printNum, device);
                Console.WriteLine("    " + printNum + ".) " + device.Name);
                printNum++;
            });
            Console.WriteLine("enter the number corresponding to the device you want to use.");
            AudioDevice selectedDevice = devices[Int32.Parse(Console.ReadLine())];

            WaveOutPlaybackTest.eVolumeControlType volumeControlType = WaveOutPlaybackTest.eVolumeControlType.DirectSampleVolume;
            Console.WriteLine("available volume control technologies are:");
            Dictionary<int, WaveOutPlaybackTest.eVolumeControlType> printedTypes = new Dictionary<int, WaveOutPlaybackTest.eVolumeControlType>();
            printNum = 1;
            Enum.GetValues(typeof(WaveOutPlaybackTest.eVolumeControlType)).OfType<WaveOutPlaybackTest.eVolumeControlType>().ToList().ForEach(volTyp => {
                printedTypes.Add(printNum, volTyp);
                Console.WriteLine("    " + printNum + ".) " + volTyp.ToString());
                printNum++;
            });
            Console.WriteLine("enter the number corresponding to the volume control technology you would like to test:");
            volumeControlType = printedTypes[Int32.Parse(Console.ReadLine())];

            // set some initial configurations
            playback.ConfigureAudioOutput(new WaveOutPlaybackConfiguration {
                selectedDevice = selectedDevice,
                VolumeControlType = volumeControlType,
                // for now we are not configuring these additional settings. come back and implement if necessary.
                NumberOfBuffers = null,
                DesiredLatency = null,
                InitialVolume = null
            });
            #endregion

            while (testingPlayback)
            {
                Console.Clear();
                if (AudioFileDuration == null)
                {
                    Console.WriteLine("ready to play audio.");
                }
                else
                {
                    Console.WriteLine("audio duration: " + TimestampToStringConverter.TimestampToString(AudioFileDuration.Value));

                    // get and display the current time information
                    TimeSpan CurrentTime;
                    playback.GetTimeInformation(out CurrentTime);
                    Console.WriteLine("current time: " + TimestampToStringConverter.TimestampToString(CurrentTime));
                }

                Console.WriteLine("enter the action you want to perform:");
                Console.WriteLine("    options are:");
                Console.WriteLine("         play, stop, quit");
                Console.WriteLine("         update: updates stats on the screen, like current time");
                Console.WriteLine("         seek: seek to a certain time in the audio file.");
                Console.WriteLine("         volume: adjust the volume of the audio.");
                switch (Console.ReadLine().ToLower())
                {
                    case "play":
                        TimeSpan Duration;
                        playback.PlayAudio(out Duration);
                        AudioFileDuration = Duration;
                        break;
                    case "stop":
                        playback.Stop();
                        AudioFileDuration = null;
                        break;
                    case "quit":
                        testingPlayback = false;
                        AudioFileDuration = null;
                        break;
                    case "seek":
                        Console.WriteLine("enter the time you want to seek to, in the format: hh mm ss");
                        string seekTimeStr = Console.ReadLine();
                        List<int> timeTokens = seekTimeStr.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Select(str => Int32.Parse(str))
                            .ToList()
                            ;
                        playback.Reposition(new TimeSpan(timeTokens[0], timeTokens[1], timeTokens[2]));
                        break;
                    case "volume":
                        Console.WriteLine("enter the volume percentage you want to set. this should be an integer value between 0 and 100:");
                        playback.SetVolume((float)(Int32.Parse(Console.ReadLine()) * .01));
                        break;
                    case "update":
                        // just do nothing. the next loop iteration will display updated info.
                        break;
                }
            }
        }
    }
}

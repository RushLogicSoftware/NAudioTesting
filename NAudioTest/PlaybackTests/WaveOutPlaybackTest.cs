using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace NAudioTest.PlaybackTests
{
    public class WaveOutPlaybackTest
    {
        public WaveOutConfigOptions GetConfigOptions()
        {
            WaveOutConfigOptions options = new WaveOutConfigOptions {
                Devices = new List<AudioDevice>()
            };


            Func<string, string> sanitize = (str) => { return new string(str.ToUpper().Where(c => char.IsLetterOrDigit(c)).ToArray()); };


            // populate list of available audio devices
            for (int a = 0; a < WaveOut.DeviceCount; a++)
            {
                WaveOutCapabilities deviceCapabilities = WaveOut.GetCapabilities(a);
                options.Devices.Add(new AudioDevice {
                    Name = deviceCapabilities.ProductName,
                    DeviceIndex = a
                });
            }

            #region see if we can improve any truncated device names by loading their full names using DirectSoundOut
            List<AudioDevice> truncatedDevices = options.Devices.Where(device => device.Name.Length >= 30).ToList();
            DirectSoundOut.Devices.ToList().ForEach(device => {
                List<AudioDevice> matchingDevices = truncatedDevices
                    .Where(existingDevice => {
                        string sanitizedExistingDevice = sanitize(existingDevice.Name);
                        string sanitizedDirectSoundDevice = sanitize(device.Description);
                        return sanitizedDirectSoundDevice.StartsWith(sanitizedExistingDevice);
                    })
                    .ToList()
                    ;
                if (matchingDevices.Count == 1)
                {
                    // we have a match
                    matchingDevices.Single().Name = device.Description;
                }
            });
            #endregion

            return options;
        }


        public WaveOutPlaybackConfiguration playbackConfig { get; set; }
        public void ConfigureAudioOutput(WaveOutPlaybackConfiguration playbackConfig)
        {
            this.playbackConfig = playbackConfig;
            this.VolumeControlType = playbackConfig.VolumeControlType;
        }

        public enum eVolumeControlType { WaveOutVolume, DirectSampleVolume };
        eVolumeControlType VolumeControlType { get; set; }

        WaveOut waveOut;
        Mp3FileReader mp3Reader;
        VolumeWaveProvider16 volumeProvider;
        public void PlayAudio(out TimeSpan Duration)
        {
            // first create an instance of IWavePlayer. in this case we will use WaveOut.
            // ** this is stored in an instance variable so we can access it later
            waveOut = new WaveOut();

            // set the playback configurations
            waveOut.DeviceNumber = playbackConfig.selectedDevice.DeviceIndex;
            if (playbackConfig.NumberOfBuffers != null)
                waveOut.NumberOfBuffers = playbackConfig.NumberOfBuffers.Value;
            if (playbackConfig.DesiredLatency != null)
                waveOut.DesiredLatency = playbackConfig.DesiredLatency.Value;
            if (VolumeControlType == eVolumeControlType.WaveOutVolume && playbackConfig.InitialVolume != null)
                waveOut.Volume = playbackConfig.InitialVolume.Value;

            // lets bind to the PlaybackStopped event of the IWavePlayer so that we can capture anytime playback stops.
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;

            // next, we need an IWaveProvider to pass into the player. in this case we will use Mp3FileReader.
            // ** this is stored in an instance variable so we can access it later
            mp3Reader = new Mp3FileReader(AudioTestingPaths.LongAudioFile);

            // report the total duration of the audio file
            Duration = mp3Reader.TotalTime;

            // pass the IWaveProvider into the Init() method of the IWavePlayer.
            switch (this.VolumeControlType)
            {
                case eVolumeControlType.WaveOutVolume:
                    waveOut.Init(mp3Reader);
                    break;
                case eVolumeControlType.DirectSampleVolume:
                    // it will first be wrapped in VolumeWaveProvider16. that step is optional, as we could have passed
                    // mp3Reader into Init(), but we want convenient sample-level volume control as well.
                    volumeProvider = new VolumeWaveProvider16(mp3Reader);
                    volumeProvider.Volume = playbackConfig.InitialVolume != null ? playbackConfig.InitialVolume.Value : 1;
                    waveOut.Init(/*mp3Reader*/volumeProvider);
                    break;
                default: throw new NotImplementedException("volume control type '" + this.VolumeControlType + "' is not yet implemented.");
            }

            // now, play
            waveOut.Play();
        }


        public void Stop()
        {
            waveOut.Stop();
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // dispose all resource we have left open.
            mp3Reader.Dispose();
            waveOut.Dispose();
        }


        public void GetTimeInformation(out TimeSpan CurrentTime)
        {
            CurrentTime = mp3Reader.CurrentTime;
        }

        public void Reposition(TimeSpan NewTime)
        {
            mp3Reader.CurrentTime = NewTime;
        }


        /// <summary>
        /// sets the playback volume
        /// </summary>
        /// <param name="volumePercentage">floating point value between 0 and 1</param>
        public void SetVolume(float volumePercentage)
        {
            switch (this.VolumeControlType)
            {
                case eVolumeControlType.WaveOutVolume:
                    waveOut.Volume = volumePercentage;
                    break;
                case eVolumeControlType.DirectSampleVolume:
                    volumeProvider.Volume = volumePercentage;
                    break;
                default: throw new NotImplementedException("volume control type '" + this.VolumeControlType + "' is not yet implemented.");
            }
        }
    }
}

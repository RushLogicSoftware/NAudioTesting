using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NAudioTest.Mixing
{
    public class RealTimeEndlessMixingTest
    {
        WaveOut waveOut { get; set; }
        MixingSampleProvider mixer { get; set; }

        public RealTimeEndlessMixingTest(int sampleRate, int channelCount)
        {
            this.waveOut = new WaveOut();
            this.mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));

            // set this as an endless mixer which will keep playing even when there is silence. start the mixer.
            mixer.ReadFully = true;
            waveOut.Init(mixer);
            waveOut.Play();

            // create a data structure for maintaining all mixer inputs. we need this when we want to remove an input.
            AllInputs = new Dictionary<string, ISampleProvider>();
        }


        Dictionary<string, ISampleProvider> AllInputs { get; set; }


        /// <summary>
        /// plays a sound through the mixer by passing in an AudioFileReader to an audio file
        /// (which does provide IEEE Floating Point samples by default).
        /// </summary>
        public void PlaySound(string inputName, string audioFilePath)
        {
            ISampleProvider input = new AudioFileReader(audioFilePath);
            AllInputs.Add(inputName, input);
            mixer.AddMixerInput(input);
        }

        /// <summary>
        /// removes the specified input from the mixer
        /// </summary>
        public void StopSound(string inputName)
        {
            if (AllInputs.ContainsKey(inputName))
            {
                ISampleProvider input = AllInputs[inputName];
                AllInputs.Remove(inputName);
                mixer.RemoveMixerInput(input);
            }
        }


        public void StopMixer()
        {
            this.mixer.RemoveAllMixerInputs();
            this.waveOut.Dispose();
            this.mixer = null;
        }
    }
}

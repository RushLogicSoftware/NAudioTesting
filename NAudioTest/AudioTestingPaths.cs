using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace NAudioTest
{
    public static class AudioTestingPaths
    {
        public static string NAudioTestingDirPath { get; set; }

        public static string LongAudioFile { get; set; }
        public static string MediumAudioFile { get; set; }

        public static string VideoFilePath { get; set; }

        public static string VoiceQuote_1 { get; set; }
        public static string VoiceQuote_2 { get; set; }
        public static string VoiceQuote_3 { get; set; }
        public static string VoiceQuote_4 { get; set; }

        public static string RecordingOutputDirPath { get; set; }
        public static string RawAudioRecordingOutputPath { get; set; }

        public static string AsioTestingDirPath { get; set; }
        public static string AsioRecordingFilePath { get; set; }

        static AudioTestingPaths()
        {
            NAudioTestingDirPath = LoadAppSetting("NAudioTestingDirPath", true);

            RecordingOutputDirPath = NAudioTestingDirPath + "Recording\\";
            RawAudioRecordingOutputPath = RecordingOutputDirPath + "RawWaveInRecording.wav";
            AsioTestingDirPath = NAudioTestingDirPath + @"Asio\";
            AsioRecordingFilePath = AsioTestingDirPath + "AsioRecording.wav";


            LongAudioFile = NAudioTestingDirPath + LoadAppSetting("LongAudioFile");
            MediumAudioFile = NAudioTestingDirPath + LoadAppSetting("MediumAudioFile");
            VideoFilePath = NAudioTestingDirPath + LoadAppSetting("VideoFile");

            VoiceQuote_1 = NAudioTestingDirPath + LoadAppSetting("VoiceQuote_1");
            VoiceQuote_2 = NAudioTestingDirPath + LoadAppSetting("VoiceQuote_2");
            VoiceQuote_3 = NAudioTestingDirPath + LoadAppSetting("VoiceQuote_3");
            VoiceQuote_4 = NAudioTestingDirPath + LoadAppSetting("VoiceQuote_4");
        }

        static string LoadAppSetting(string appsettingKey, bool isDirectoryPath = false)
        {
            string settingValue = ConfigurationManager.AppSettings[appsettingKey];

            if (string.IsNullOrEmpty(settingValue))
                throw new Exception("required setting key '" + appsettingKey + "' was not set in App.config.");

            if (isDirectoryPath && settingValue[settingValue.Length - 1] != '\\')
                settingValue += "\\";

            return settingValue;
        }
    }
}

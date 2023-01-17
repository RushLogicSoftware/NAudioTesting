using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio;
using NAudio.MediaFoundation;
using NAudio.Wave;

namespace NAudioTest.Codecs.MFT
{

    public enum eMFTAudioSubtype {
        ImaAdpcm,
        KSDATAFORMAT_SUBTYPE_ADPCM,
        KSDATAFORMAT_SUBTYPE_IEC61937_DOLBY_DIGITAL_PLUS,
        KSDATAFORMAT_SUBTYPE_MULAW,
        MEDIASUBTYPE_DOLBY_DDPLUS,
        MEDIASUBTYPE_DVM,
        MEDIASUBTYPE_MSAUDIO1,
        MEDIASUBTYPE_RAW_AAC1,
        MFAudioFormat_AAC,
        MFAudioFormat_ADTS,
        MFAudioFormat_Dolby_AC3,
        MFAudioFormat_Dolby_AC3_SPDIF,
        MFAudioFormat_DRM,
        MFAudioFormat_DTS,
        MFAudioFormat_Float,
        MFAudioFormat_MP3,
        MFAudioFormat_MPEG,
        MFAudioFormat_MSP1,
        MFAudioFormat_PCM,
        MFAudioFormat_WMASPDIF,
        MFAudioFormat_WMAudioV8,
        MFAudioFormat_WMAudioV9,
        MFAudioFormat_WMAudio_Lossless,
        WMMEDIASUBTYPE_WMSP2
    };

    public enum eAvailableOutputMediaTypeQueryResult {
        UNKNOWN,
        TRANSFORMS_FOUND,

        // MFT returned error: "No suitable transform was found to encode or decode the content"
        NO_SUITABLE_TRANSFORM_FOUND
    };

    public class MftCapabilityChecker
    {



        public List<int> GetAvailableBitratesForAudioSubtype(eMFTAudioSubtype audioSubtype, int sampleRate, int channelCount)
        {
            List<int> availableBitrates = MediaFoundationEncoder.GetEncodeBitrates(
                GetAudioSubtypeGUID(audioSubtype),
                sampleRate,
                channelCount
                ).ToList();
            return availableBitrates;
        }


        public eAvailableOutputMediaTypeQueryResult GetAvailableOutputMediaTypesForAudioSubtype(
            eMFTAudioSubtype audioSubtype, out List<MediaTypeWrapper> mediaTypes
            )
        {
            eAvailableOutputMediaTypeQueryResult result = eAvailableOutputMediaTypeQueryResult.UNKNOWN;
            mediaTypes = null;

            try
            {
                mediaTypes = MediaFoundationEncoder.GetOutputMediaTypes(GetAudioSubtypeGUID(audioSubtype))
                    .Select(mt => {
                        MediaTypeWrapper wrapper = new MediaTypeWrapper {
                            MajorType = mt.MajorType.ToString(),
                            SubType = mt.SubType.ToString(),
                            AverageBytesPerSecond = -1,
                            BitsPerSample = -1,
                            ChannelCount = -1,
                            SampleRate = -1
                        };

                        // set these values in catch blocks becasue they are not always supported by the underlying format
                        try { wrapper.AverageBytesPerSecond = mt.AverageBytesPerSecond; } catch { }
                        try { wrapper.BitsPerSample = mt.BitsPerSample; } catch { }
                        try { wrapper.ChannelCount = mt.ChannelCount; } catch { }
                        try { wrapper.SampleRate = mt.SampleRate; } catch { }

                        return wrapper;
                    })
                    .ToList()
                    ;
                result = eAvailableOutputMediaTypeQueryResult.TRANSFORMS_FOUND;
            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("No suitable transform was found to encode or decode the content"))
                {
                    result = eAvailableOutputMediaTypeQueryResult.NO_SUITABLE_TRANSFORM_FOUND;
                }
                else
                    throw exc;
            }

            return result;
        }
        public class MediaTypeWrapper
        {
            public string MajorType { get; set; }
            public string SubType { get; set; }

            public int AverageBytesPerSecond { get; set; }
            public int BitsPerSample { get; set; }
            public int ChannelCount { get; set; }
            public int SampleRate { get; set; }
        }


        public static Guid GetAudioSubtypeGUID(eMFTAudioSubtype audioSubtype)
        {
            switch (audioSubtype)
            {
                case eMFTAudioSubtype.ImaAdpcm:
                    return AudioSubtypes.ImaAdpcm;
                case eMFTAudioSubtype.KSDATAFORMAT_SUBTYPE_ADPCM:
                    return AudioSubtypes.KSDATAFORMAT_SUBTYPE_ADPCM;
                case eMFTAudioSubtype.KSDATAFORMAT_SUBTYPE_IEC61937_DOLBY_DIGITAL_PLUS:
                    return AudioSubtypes.KSDATAFORMAT_SUBTYPE_IEC61937_DOLBY_DIGITAL_PLUS;
                case eMFTAudioSubtype.KSDATAFORMAT_SUBTYPE_MULAW:
                    return AudioSubtypes.KSDATAFORMAT_SUBTYPE_MULAW;
                case eMFTAudioSubtype.MEDIASUBTYPE_DOLBY_DDPLUS:
                    return AudioSubtypes.MEDIASUBTYPE_DOLBY_DDPLUS;
                case eMFTAudioSubtype.MEDIASUBTYPE_DVM:
                    return AudioSubtypes.MEDIASUBTYPE_DVM;
                case eMFTAudioSubtype.MEDIASUBTYPE_MSAUDIO1:
                    return AudioSubtypes.MEDIASUBTYPE_MSAUDIO1;
                case eMFTAudioSubtype.MEDIASUBTYPE_RAW_AAC1:
                    return AudioSubtypes.MEDIASUBTYPE_RAW_AAC1;
                case eMFTAudioSubtype.MFAudioFormat_AAC:
                    return AudioSubtypes.MFAudioFormat_AAC;
                case eMFTAudioSubtype.MFAudioFormat_ADTS:
                    return AudioSubtypes.MFAudioFormat_ADTS;
                case eMFTAudioSubtype.MFAudioFormat_Dolby_AC3:
                    return AudioSubtypes.MFAudioFormat_Dolby_AC3;
                case eMFTAudioSubtype.MFAudioFormat_Dolby_AC3_SPDIF:
                    return AudioSubtypes.MFAudioFormat_Dolby_AC3_SPDIF;
                case eMFTAudioSubtype.MFAudioFormat_DRM:
                    return AudioSubtypes.MFAudioFormat_DRM;
                case eMFTAudioSubtype.MFAudioFormat_DTS:
                    return AudioSubtypes.MFAudioFormat_DTS;
                case eMFTAudioSubtype.MFAudioFormat_Float:
                    return AudioSubtypes.MFAudioFormat_Float;
                case eMFTAudioSubtype.MFAudioFormat_MP3:
                    return AudioSubtypes.MFAudioFormat_MP3;
                case eMFTAudioSubtype.MFAudioFormat_MPEG:
                    return AudioSubtypes.MFAudioFormat_MPEG;
                case eMFTAudioSubtype.MFAudioFormat_MSP1:
                    return AudioSubtypes.MFAudioFormat_MSP1;
                case eMFTAudioSubtype.MFAudioFormat_PCM:
                    return AudioSubtypes.MFAudioFormat_PCM;
                case eMFTAudioSubtype.MFAudioFormat_WMASPDIF:
                    return AudioSubtypes.MFAudioFormat_WMASPDIF;
                case eMFTAudioSubtype.MFAudioFormat_WMAudioV8:
                    return AudioSubtypes.MFAudioFormat_WMAudioV8;
                case eMFTAudioSubtype.MFAudioFormat_WMAudioV9:
                    return AudioSubtypes.MFAudioFormat_WMAudioV9;
                case eMFTAudioSubtype.MFAudioFormat_WMAudio_Lossless:
                    return AudioSubtypes.MFAudioFormat_WMAudio_Lossless;
                case eMFTAudioSubtype.WMMEDIASUBTYPE_WMSP2:
                    return AudioSubtypes.WMMEDIASUBTYPE_WMSP2;
                default: throw new NotImplementedException("obtaining a GUID for audio subtype " + audioSubtype + " is not yet implemented.");
            }
        }

    }
}

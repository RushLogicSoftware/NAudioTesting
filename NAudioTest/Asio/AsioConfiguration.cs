using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest.Asio
{
    public enum eInputChannelSelection { AllChannels, SpecificRange };
    public class AsioConfiguration
    {
        public AsioDriver SelectedDriver { get; set; }

        #region recording
        public eSampleRate recordingSampleRate { get; set; }
        public eBitDepth recordingBitDepth { get; set; }

        public eInputChannelSelection inputChannelSelection { get; set; }

        public int? ChannelRange_FirstChannel { get; set; }
        public int? ChannelRange_LastChannel { get; set; }
        #endregion
    }
}

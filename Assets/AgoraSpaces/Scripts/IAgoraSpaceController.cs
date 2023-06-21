using System;
using Agora.Rtc;

namespace Agora.Spaces
{
    public interface IAgoraSpaceController
    {
        string ChannelID { get; }
        bool JoinedChannel { get; }
        uint LocalUID { get; }

        event Action<uint> OnOfflineNotify;

        void JoinChannel(uint uid);
        void LeaveChannel();
        void MuteCamera(bool mute);
        void MuteMic(bool mute);
        void SetChannel(string channelName);
        int UpdateRemotePosition(uint uid, RemoteVoicePositionInfo posInfo);
        int UpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp);
        void updateSpatialAudioPosition(uint remoteUid, float sourceDistance);
    }
}
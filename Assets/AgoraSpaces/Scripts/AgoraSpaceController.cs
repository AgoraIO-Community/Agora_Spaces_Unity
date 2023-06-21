using UnityEngine;
using UnityEngine.UI;
using Agora.Rtc;
using AgoraSTTSample.Utility;
using AgoraSTTSample.Models;
using Agora_RTC_Plugin.API_Example;

namespace Agora.Spaces
{
    public class AgoraSpaceController : MonoBehaviour, IAgoraSpaceController
    {
        public static AgoraSpaceController Instance { get; private set; }

        [Header("_____________Basic Configuration_____________")]
        [SerializeField]
        private AppIdInput _appIdInput;

        private string _appID = "";

        private string _token = "";

        private string _channelName = "";

        [SerializeField]
        bool VideoStreamWanted = true;
        [SerializeField]
        internal string MediaPlayerContainer = "MediaPlayer";

        internal IRtcEngineEx RtcEngine { get; private set; }
        internal ILocalSpatialAudioEngine SpatialAudioController { get; private set; }

        public string ChannelID { get { return _channelName; } }
        public bool JoinedChannel { get; internal set; }
        public uint LocalUID { get; internal set; }
        public event System.Action<uint> OnOfflineNotify;
        public event System.Action<uint> OnJoinedChannelNotify;

        private AgoraRTTController _rttController;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;
            if (transform.parent == null) DontDestroyOnLoad(this);
        }

        // Use this for initialization
        private void Start()
        {
            LoadAssetData();
            if (CheckAppId())
            {
                InitRTCEngine();
                InitSpatialAudioEngine();
                _rttController = new AgoraRTTController();
                _rttController.InitSTTManager(_appID, _channelName);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            PermissionHelper.RequestMicrophontPermission();
            PermissionHelper.RequestCameraPermission();
        }


        //Show data in AgoraBasicProfile
        [ContextMenu("ShowAgoraBasicProfileData")]
        private void LoadAssetData()
        {
            if (_appIdInput == null) return;
            _appID = _appIdInput.appID;
            _token = _appIdInput.token;
            _channelName = _appIdInput.channelName;
        }

        private bool CheckAppId()
        {
            Debug.Assert(_appID.Length > 10, "Please fill in your appId in Assets/AgoraSpaces/_AppIDInput/AppIdInput.asset");
            return _appID.Length > 10;
        }

        private void InitRTCEngine()
        {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngineEx();
            UserEventHandler handler = new UserEventHandler(this);

            //If you use a Bluetooth headset, you need to set AUDIO_SCENARIO_TYPE to AUDIO_SCENARIO_GAME_STREAMING.
            RtcEngineContext context = new RtcEngineContext(_appID, 0,
                                        CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                        AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_GAME_STREAMING);
            RtcEngine.Initialize(context);

            RtcEngine.EnableVideo();
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            RtcEngine.InitEventHandler(handler);
        }

        private void InitSpatialAudioEngine()
        {
            // By default Agora subscribes to the audio streams of all remote users.
            // Unsubscribe all remote users; otherwise, the audio reception range you set
            // is invalid.
            RtcEngine.MuteAllRemoteAudioStreams(true);
            RtcEngine.EnableAudio();

            // RtcEngine.EnableSpatialAudio(true);
            LocalSpatialAudioConfig localSpatialAudioConfig = new LocalSpatialAudioConfig();
            localSpatialAudioConfig.rtcEngine = RtcEngine;

            // RtcEngine.EnableSpatialAudio(true);
            SpatialAudioController = RtcEngine.GetLocalSpatialAudioEngine();
            var ret = SpatialAudioController.Initialize();
            Debug.Log("_spatialAudioEngine: Initialize " + ret);
            // Set the audio reception range, in meters, of the local user
            SpatialAudioController.SetAudioRecvRange(50);
            // Set the length, in meters, of unit distance
            SpatialAudioController.SetDistanceUnit(1);
        }

        public void SetChannel(string channelName)
        {
            this._channelName = channelName;
        }

        // RtcConnection _rtcConnection = new RtcConnection();
        public void JoinChannel(uint uid)
        {
            if (JoinedChannel) return;

            ChannelMediaOptions options = new ChannelMediaOptions();
            options.autoSubscribeAudio.SetValue(true);
            options.autoSubscribeVideo.SetValue(VideoStreamWanted);
            options.publishCameraTrack.SetValue(VideoStreamWanted);
            options.publishMicrophoneTrack.SetValue(true);
            options.enableAudioRecordingOrPlayout.SetValue(true);
            options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            RtcEngine.JoinChannel(_token, _channelName, uid, options);
        }

        public int UpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp)
        {
            RtcConnection connection = new RtcConnection(ChannelID, LocalUID);
            var ret = SpatialAudioController.UpdateSelfPosition(position, axisForward, axisRight, axisUp);
            return ret;
        }
        public int UpdateRemotePosition(uint uid, RemoteVoicePositionInfo posInfo)
        {
            RtcConnection connection = new RtcConnection(ChannelID, LocalUID);
            var ret = SpatialAudioController.UpdateRemotePosition(uid, posInfo);
            return ret;
        }

        public void LeaveChannel()
        {
            RtcEngine.LeaveChannel();
            SpatialAudioController.Dispose();
            JoinedChannel = false;
        }

        public void MuteMic(bool mute)
        {
            RtcEngine.MuteLocalAudioStream(mute);
        }

        public void MuteCamera(bool mute)
        {
            RtcEngine.MuteLocalVideoStream(mute);
            RtcEngine.EnableLocalVideo(!mute);
        }

        internal void HandleJoinedChannel(uint user)
        {
            OnJoinedChannelNotify?.Invoke(user);
        }

        internal void HandleOffline(uint user)
        {
            OnOfflineNotify?.Invoke(user);
        }

        internal void HandleStopSTT()
        {
            _rttController.HandleStopSTT();
        }

        internal void HandleStartSTT(Text txt)
        {
            _rttController.HandleStartSTT(txt);
        }

        internal void SetSTTLanguage(STTLangEnum lang)
        {
            _rttController.STT_Language = lang;
        }

        public void updateSpatialAudioPosition(uint remoteUid, float sourceDistance)
        {
            // remoteUid = GameApplication.LastRemoteUser;
            // Set the coordinates in the world coordinate system.
            // This parameter is an array of length 3
            // The three values represent the front, right, and top coordinates
            float[] position = new float[] { sourceDistance, 4.0F, 0.0F };
            // Set the unit vector of the x axis in the coordinate system.
            // This parameter is an array of length 3,
            // The three values represent the front, right, and top coordinates
            float[] forward = new float[] { sourceDistance, 0.0F, 0.0F };
            // Update the spatial position of the specified remote user
            RemoteVoicePositionInfo remotePosInfo = new RemoteVoicePositionInfo(position, forward);
            var rc = UpdateRemotePosition(remoteUid, remotePosInfo);
            // Debug.Log($"Remote user ${remoteUid} spatial position updated(${sourceDistance} rc = " + rc);
        }

        private void OnDestroy()
        {
            if (RtcEngine == null) return;

            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
            RtcEngine = null;
            Instance = null;
            Destroy(gameObject);
        }

        internal MediaTV GetMediaTV()
        {
            string MediaPlayerContainer = AgoraSpaceController.Instance.MediaPlayerContainer;
            var mediaTV = GameObject.Find(MediaPlayerContainer)?.GetComponent<MediaTV>();
            return mediaTV;
        }

        // ======================= Required EventHandler for Agora RTC events ======================
        internal class UserEventHandler : IRtcEngineEventHandler
        {
            private readonly AgoraSpaceController app;

            internal UserEventHandler(AgoraSpaceController obj)
            {
                app = obj;
            }

            public override void OnError(int err, string msg)
            {
                Debug.LogError(string.Format("OnError err: {0}, msg: {1}", err, msg));
            }

            public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
            {
                Debug.Log(string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                                    connection.channelId, connection.localUid, elapsed));
                app.JoinedChannel = true;
                app.LocalUID = connection.localUid;
                app.HandleJoinedChannel(app.LocalUID);

                float[] localUserPosition = new float[] { 0.0f, 0.0f, 0.0f };
                float[] forward = new float[] { 1.0f, 0.0f, 0.0f };
                float[] right = new float[] { 0.0f, 1.0f, 0.0f };
                float[] up = new float[] { 0.0f, 0.0f, 1.0f };
                var ret = app.SpatialAudioController.UpdateSelfPosition(localUserPosition, forward, right, up);
                Debug.Log("UpdateSelfPosition return: " + ret);
            }

            public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
            {
                app.LocalUID = connection.localUid;
                Debug.Log("OnRejoinChannelSuccess， uid:" + app.LocalUID);
            }

            public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
            {
                Debug.Log("OnLeaveChannel");
            }

            public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
            {
                Debug.Log($"OnClientRoleChanged, old:{oldRole} new:{newRole}");
            }

            public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
            {
                Debug.Log(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
                var mediaTV = app.GetMediaTV();
                if (mediaTV != null && uid == mediaTV.UidUseInMPK && !GameApplication.Instance.IsHost)
                {
                    mediaTV.DisplayStreamFromUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                }
            }

            public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
            {
                Debug.Log(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                    (int)reason));
                app.HandleOffline(uid);
                var mediaTV = app.GetMediaTV();
                if (mediaTV != null && uid == mediaTV.UidUseInMPK && !GameApplication.Instance.IsHost)
                {
                    mediaTV.StopStreamDisplay();
                }
            }

            // Receiving data stream text from STT remote service
            public override void OnStreamMessage(RtcConnection connection, uint remoteUid, int streamId, byte[] data, uint length, ulong sentTs)
            {
                //Debug.Log(String.Format("remoteUid: {0}", remoteUid));
                // if (remoteUid == (int)app.LocalUID + app.STT_Datastream_ID)
                {
                    AgoraSTTSample.Protobuf.Text t = ProtobufUtility.ParseProtobufData(data);
                    app._rttController.STTTextHandler(t);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Agora.Rtc;

namespace Agora.Spaces
{
    [RequireComponent(typeof(NetworkTransform))]
    public class SpacePlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetPlayerID))]
        internal uint player_uid;

        [SyncVar(hook = nameof(SetPlayerName))]
        internal string player_name;

        [SerializeField]
        Text nameLabel;

        public static readonly Dictionary<uint, SpacePlayer> PlayerMap = new Dictionary<uint, SpacePlayer>();

        public event System.Action OnPlayerPositionChanged;

        void OnPlayerPositionChangedHandler()
        {
            var position = transform.position;
            var forward = transform.forward;
            if (this.isLocalPlayer)
            {
                float[] forwardLocal = { forward.x, forward.y, forward.z };
                float[] positionLocal = { position.x, position.y, position.z };
                float[] right = { transform.right.x, transform.right.y, transform.right.z };
                float[] up = { transform.up.x, transform.up.y, transform.up.z };
                if (AgoraSpaceController.Instance.SpatialAudioController != null)
                {
                    var rc = AgoraSpaceController.Instance.UpdateSelfPosition(positionLocal, forwardLocal, right, up);
                }
            }
            else
            {
                float[] positionRemote = { position.x, position.y, position.z };
                float[] forwardRemote = { forward.x, forward.y, forward.z };
                if (AgoraSpaceController.Instance.SpatialAudioController != null)
                {
                    var rc = AgoraSpaceController.Instance.UpdateRemotePosition(player_uid, new RemoteVoicePositionInfo(positionRemote, forwardRemote));
                }
            }
        }

        void SetPlayerID(uint old_id, uint new_id)
        {
            Debug.Log($"SyncHook SetPlayerID:{old_id} -> {new_id}");
        }

        void SetPlayerName(string old_name, string new_name)
        {
            Debug.Log($"SyncHook SetPlayerName:{old_name} -> {new_name}");
            nameLabel.text = new_name;
        }

        #region AgoraNetworkEvent
        //override NetworkBehaviour
        public override void OnStopServer()
        {
            CancelInvoke();
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                AgoraSpaceController.Instance.OnJoinedChannelNotify += CmdHandleJoinedChannel;
            }
            // Debug.Log("OnStartClient for " + (isLocalPlayer ? "Local:" : "Remote:") + player_uid);
            if (player_uid != 0)
            {
                PlayerMap[player_uid] = this;
            }

            OnPlayerPositionChanged += OnPlayerPositionChangedHandler;

            OnPlayerPositionChanged();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Assert(GameApplication.Instance != null, "Null GameApplication!");
            CmdSetPlayerName(GameApplication.Instance.UserName);
        }

        [Command]
        internal void CmdSetPlayerName(string pname)
        {
            player_name = pname;
            nameLabel.text = pname;
        }

        #endregion
        [Command]
        void CmdHandleJoinedChannel(uint uid)
        {
            player_uid = uid;
        }

        Vector3 _oldPos { get; set; }
        Vector3 _oldFwd { get; set; }

        private void Start()
        {
            _oldFwd = this.transform.forward;
            _oldPos = this.transform.position;
        }


        // Update is called once per frame
        void Update()
        {
            if (_oldPos != transform.position || _oldFwd != transform.forward)
            {
                OnPlayerPositionChanged();
                _oldFwd = this.transform.forward;
                _oldPos = this.transform.position;
            }
        }

        public static Color32 GetColor(uint player_id)
        {
            Color32 color = Color.white;
            if (PlayerMap.ContainsKey(player_id))
            {
                var player = PlayerMap[player_id];
                var rc = player.GetComponent<RandomColor>();
                if (rc != null)
                {
                    color = rc.color;
                }
            }
            return color;
        }
    }
}

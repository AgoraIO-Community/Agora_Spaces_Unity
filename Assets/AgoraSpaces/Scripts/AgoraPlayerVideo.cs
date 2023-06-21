using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Agora.Rtc;

namespace Agora.Spaces
{
    public class AgoraPlayerVideo : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        uint _uid;

        NetworkBehaviour _network;
        SpacePlayer _spacePlayer;

        bool _isLocalPlayer = false;

        System.Collections.IEnumerator Start()
        {
            while (_network == null)
            {
                yield return null;
                _network = transform.parent.GetComponent<NetworkBehaviour>();
            }

            // SpacePlayer component has the uid stored
            _isLocalPlayer = _network.isLocalPlayer;
            // save the network interfacing controller
            _spacePlayer = _network.GetComponent<SpacePlayer>();

            var app = AgoraSpaceController.Instance;
            app.OnOfflineNotify += HandleOffline;

            yield return new WaitUntil(() => app.JoinedChannel);
            StartCoroutine(EnableVideoDisplay());
        }

        IEnumerator EnableVideoDisplay()
        {
            var videosurface = GetComponent<VideoSurface>();
            if (videosurface == null)
            {
                videosurface = gameObject.AddComponent<VideoSurface>();
            }
            if (_isLocalPlayer)
            {
                videosurface.SetForUser(0, AgoraSpaceController.Instance.ChannelID);
            }
            else
            {
                // for remote player, make sure a valid uid is updated via synvar
                yield return new WaitUntil(() => _spacePlayer.player_uid != 0);
                _uid = _spacePlayer.player_uid;
                videosurface.SetForUser(_uid, AgoraSpaceController.Instance.ChannelID, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            }
            videosurface.SetEnable(true);
        }

        void HandleOffline(uint offline_uid)
        {
            if (this != null && _uid != AgoraSpaceController.Instance.LocalUID && _uid == offline_uid)
            {
                Destroy(gameObject);
            }
        }
    }
}

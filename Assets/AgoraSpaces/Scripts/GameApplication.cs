using UnityEngine;
using Mirror;
namespace Agora.Spaces
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField]
        LoginPrompt Login;

        [SerializeField]
        NetworkManager Manager;

        public string UserName { get; internal set; }
        public bool IsHost { get; internal set; }

        public static GameApplication Instance;


        public event System.Action OnGameStop;
        public event System.Action<string> OnSceneChangeNotify;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;
            if (transform.parent == null) DontDestroyOnLoad(this);
            Login.Play += StartGame;
        }

        public void NotifySceneChange(string scene)
        {
            OnSceneChangeNotify?.Invoke(scene);
        }

        void StartGame(IEntryInfo info)
        {
            UserName = info.UserName;
            IsHost = info.IsHost;
            // This updates networkAddress every frame from the TextField
            Manager.networkAddress = info.NetworkAddress;
            AgoraSpaceController.Instance.SetChannel(info.RoomName);
            if (info.IsHost)
            {
                Manager.StartHost();
            }
            else
            {
                Manager.StartClient();
            }
        }

        public void StopGame()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Manager.StopHost();
            }
            // client only
            else if (NetworkClient.isConnected)
            {
                Manager.StopClient();
            }

            OnGameStop?.Invoke();
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agora.Spaces
{
    public class LoginPrompt : MonoBehaviour, IEntryInfo
    {
        [SerializeField]
        InputField UserNameInput;

        [SerializeField]
        InputField ServerAddressInput;

        [SerializeField]
        InputField GameRoomNameInput;

        [SerializeField]
        Dropdown IdentityDropdown;

        public string UserName
        {
            get { return UserNameInput.text; }
            set { UserNameInput.text = value; }
        }
        public string NetworkAddress
        {
            get { return ServerAddressInput.text; }
            set { ServerAddressInput.text = value; }
        }

        public string RoomName
        {
            get { return GameRoomNameInput.text; }
            set { GameRoomNameInput.text = value; }
        }

        public string Identity { get { return IdentityList[IdentityDropdown.value]; } }
        public bool IsHost { get; set; }

        public event System.Action<IEntryInfo> Play;

        private List<string> IdentityList = new List<string> { "Client", "Host (Client+Server)" };

        private void Awake()
        {
            int DropdownChoice = PlayerPrefs.GetInt("NetChoice", 0);
            ServerAddressInput.text = PlayerPrefs.GetString("ServerAddress", "localhost");
            var roomName = PlayerPrefs.GetString("RoomName", "");
            if (roomName != "")
            {
                GameRoomNameInput.text = roomName;
            }

            var username = PlayerPrefs.GetString("UserName", "");
            if (username != "")
            {
                UserNameInput.text = username;
            }

            IdentityDropdown.ClearOptions();
            IdentityDropdown.AddOptions(IdentityList);
            IdentityDropdown.value = DropdownChoice;
        }

        public void OnPlayButton()
        {
            IsHost = (IdentityDropdown.value == 1);
            PlayerPrefs.SetString("ServerAddress", ServerAddressInput.text);
            PlayerPrefs.SetString("UserName", UserNameInput.text);
            PlayerPrefs.SetString("RoomName", GameRoomNameInput.text);
            PlayerPrefs.SetInt("NetChoice", IdentityDropdown.value);
            PlayerPrefs.Save();
            if (string.IsNullOrEmpty(RoomName))
            {
                Debug.LogWarning("RoomName can not be empty!");
                return;
            }
            Play?.Invoke(this);
        }
    }
}

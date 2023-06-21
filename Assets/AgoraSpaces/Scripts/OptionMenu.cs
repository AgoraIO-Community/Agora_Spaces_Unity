using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Agora.Rtc;
using Agora.Util;
using Mirror;
using AgoraSTTSample.Models;

namespace Agora.Spaces
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] Button MenuButton;
        [SerializeField] Button ExitButton;
        [SerializeField] ToggleStateButton MuteMicButton;
        [SerializeField] ToggleStateButton MuteCamButton;
        [SerializeField] ToggleStateButton PlayButton;
        [SerializeField] ToggleStateButton SttButton;
        // [SerializeField] Button WebButton;
        [SerializeField] GameObject MenuDialog;
        [SerializeField] GameObject STTView;
        [SerializeField] Text STTSubtitle;

        [SerializeField] Dropdown LanguageDropdown;

        [SerializeField] string WebURL;

        IRtcEngine _rtcEngine;
        // Start is called before the first frame update
        IEnumerator Start()
        {
            MenuDialog.SetActive(false);
            STTView.SetActive(false);
            MuteCamButton.gameObject.SetActive(false);
            MuteMicButton.gameObject.SetActive(false);
            MenuButton.onClick.AddListener(HandleMenuTap);
            ExitButton.onClick.AddListener(HandleExit);
            yield return new WaitUntil(() => AgoraSpaceController.Instance.RtcEngine != null);
            _rtcEngine = AgoraSpaceController.Instance.RtcEngine;
            SetupLangSelection();
            SetupButtons();
        }

        void SetupLangSelection()
        {
            var names = new List<string>(System.Enum.GetNames(typeof(STTLangEnum)));
            LanguageDropdown.ClearOptions();
            LanguageDropdown.AddOptions(names);
            LanguageDropdown.value = (int)STTLangEnum.en_US;
        }

        // Called as a button event
        public void ConfirmLangSelection()
        {
            int sel = LanguageDropdown.value;
            AgoraSpaceController.Instance.SetSTTLanguage((STTLangEnum)sel);
        }

        void SetupButtons()
        {
            MuteMicButton.Setup(initOnOff: false,
                onStateText: "Mute Mic", offStateText: "Unmute Mic",
                callOnAction: () =>
                {
                    AgoraSpaceController.Instance.MuteMic(true);
                },
                callOffAction: () =>
                {
                    AgoraSpaceController.Instance.MuteMic(false);
                }
            );
            MuteCamButton.Setup(initOnOff: false,
                onStateText: "Disable WebCam", offStateText: "Enable WebCam",
                callOnAction: () =>
                {
                    Debug.Log("Local Camera muted");
                    AgoraSpaceController.Instance.MuteCamera(true);

                },
                callOffAction: () =>
                {
                    Debug.Log("Local Camera enabled");
                    AgoraSpaceController.Instance.MuteCamera(false);
                }
            );

            PlayButton.Setup(initOnOff: false,
                onStateText: "Play Media", offStateText: "Stop Media",
                callOnAction: () =>
                {
                    Debug.Log("Play media");
                    PlayMedia(true);
                    MenuDialog.SetActive(false);
                },
                callOffAction: () =>
                {
                    Debug.Log("Stop media");
                    PlayMedia(false);
                    MenuDialog.SetActive(false);
                }
            );
            SttButton.Setup(initOnOff: false,
                onStateText: "Enable STT", offStateText: "Disable STT",
                callOnAction: () =>
                {
                    Debug.Log("Start STT");
                    MenuDialog.SetActive(false);
                    STTView.SetActive(true);
                    AgoraSpaceController.Instance.HandleStartSTT(STTSubtitle);
                },
                callOffAction: () =>
                {
                    Debug.Log("Stop STT");
                    STTView.SetActive(false);
                    MenuDialog.SetActive(false);
                    AgoraSpaceController.Instance.HandleStopSTT();
                }
);

            MuteCamButton.gameObject.SetActive(true);
            MuteMicButton.gameObject.SetActive(true);
            PlayButton.gameObject.SetActive(true);
            PlayButton.GetComponent<Button>().interactable = GameApplication.Instance.IsHost;

            //WebButton.onClick.AddListener(() => Application.OpenURL(WebURL));
        }

        void HandleMenuTap()
        {
            MenuDialog.SetActive(!MenuDialog.activeInHierarchy);
        }

        void HandleExit()
        {
            GameApplication.Instance.StopGame();
        }

        void PlayMedia(bool play)
        {
            var mediaTV = AgoraSpaceController.Instance.GetMediaTV();
            if (mediaTV != null)
            {
                if (play) { mediaTV.Play(); }
                else { mediaTV.Stop(); }
            }
        }
    }
}

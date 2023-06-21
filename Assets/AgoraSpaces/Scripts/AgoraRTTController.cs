using System;
using System.Collections;
using System.Collections.Generic;
using AgoraSTTSample;
using AgoraSTTSample.Models;
using AgoraSTTSample.Utility;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Types;

namespace Agora.Spaces
{
    public class AgoraRTTController : IDisposable
    {

        internal STTManager STTMgr;

        private string _sttToken = "";
        private string _sttTaskId = "";
        public bool _isSTTStarted = false;

        internal int STT_Audio_ID = 980;
        internal int STT_Datastream_ID = 981;
        internal STTLangEnum STT_Language { get; set; } = STTLangEnum.en_US;

        private Text SubtitleText;

        private List<STTSubtitle> SubtitleList = new List<STTSubtitle>();
        private List<int> STTUserOrder = new List<int>();
        Dictionary<int, STTSubtitle> SubtitleRec = new Dictionary<int, STTSubtitle>();
        int LastSTTUid { get; set; } = -1;

        bool JoinedChannel => AgoraSpaceController.Instance.JoinedChannel;
        string _channelName;

        public void InitSTTManager(string appID, string channelName)
        {
            this.STTMgr = STTManager.Instance;
            this.STTMgr.appId = appID;
            this._channelName = channelName;
            LoadSTTInfo();
        }

        public void Dispose()
        {
            SaveSTTInfo();
            HandleStopSTT();
        }

        internal void HandleStopSTT()
        {
            if (_isSTTStarted)
            {
                this.StopSTTService();
            }
        }

        internal void HandleStartSTT(Text txt)
        {
            SubtitleText = txt;
            if (JoinedChannel && !_isSTTStarted && SubtitleText != null)
            {
                this.StartSTTService();
            }
        }


        private void SaveSTTInfo()
        {
            PlayerPrefs.SetString("STT_TaskID", _sttTaskId);
            PlayerPrefs.SetString("STT_Token", _sttToken);
            PlayerPrefs.Save();
        }

        private void LoadSTTInfo()
        {
            _sttToken = PlayerPrefs.GetString("STT_Token", "");
            _sttTaskId = PlayerPrefs.GetString("STT_TaskID", "");
        }

        // STT
        private async void StartSTTService()
        {
            string token = await this.STTMgr.Acquire(this._channelName);
            if (token.Length <= 0)
            {
                Debug.LogError("Fail to get STT token.");
                return;
            }
            string logText = String.Format("Acquire STT token success, token: {0}", token);
            Debug.Log(logText);

            this._sttToken = token;
            this.STTMgr.STTLang = this.STT_Language;
            string taskId = await this.STTMgr.Start(token, _channelName, STT_Audio_ID, STT_Datastream_ID);
            if (taskId.Length <= 0)
            {
                Debug.LogError("Fail in start STT task.");
                return;
            }

            logText = String.Format("Start STT task success, taskId: {0}", taskId);
            Debug.Log(logText);
            this._sttTaskId = taskId;

            if (this._sttTaskId.Length > 0)
            {
                this._isSTTStarted = true;
            }
        }

        private async void QuerySTTService()
        {
            AgoraSTTSample.Models.STTQueryResponseModel model = await this.STTMgr.Query(_sttTaskId, _sttToken);
            if (model != null)
            {
                Debug.Log($"STT Query result: {model.status}");
            }
        }

        private async void StopSTTService()
        {
            bool result = await this.STTMgr.Stop(_sttTaskId, _sttToken);
            Debug.Log(String.Format("Stop STT: {0}", result));
            if (result)
            {
                this._isSTTStarted = false;
            }
        }

        class STTSubtitle
        {
            internal int Uid { get; set; }
            internal string Text { get; set; }
            internal bool isFinal { get; set; }

            internal void Update(string text, bool isFinal)
            {
                this.Text = text;
                this.isFinal = isFinal;
            }
        }

        internal void STTTextHandler(AgoraSTTSample.Protobuf.Text text)
        {
            //Debug.Log(string.Format("STTTextHandler start....{0}", text.Words.Last().IsFinal));
            (string retStr, string retConfinece, string wholeText, int finalLength, string currentFinalText)
                = ProtobufUtility.createStringWithText(text, delegate (string finalText, string finalTextConfidence)
                {
                    //string logStr = string.Format("\nfinalText: {0}, \nfinalTextConfidence: {1}", finalText, finalTextConfidence);
                    //Log.UpdateLog(logStr);
                    Debug.LogWarning("finalText handler....");
                    this.ReceiveFinalText(finalText, text.Uid);
                });

            //string logStr
            //    = string.Format(
            //        "\nretStr: {0} \nretConfinece: {1} \nwholeText: {2} \nfinalLength: {3} \ncurrentFinalText: {4}",
            //        retStr, retConfinece, wholeText, finalLength, currentFinalText);
            //Log.UpdateLog(logStr);

            this.ReceiveText(retStr, text.Uid);
        }

        void ReceiveText(string text, int textUid)
        {
            //Debug.Log(string.Format("ReceiveText {0}，from {1}", text, textUid));
            if (string.IsNullOrEmpty(text)) return;

            STTSubtitle newSubtitle = new STTSubtitle();
            newSubtitle.Uid = textUid;
            newSubtitle.Text = text;
            newSubtitle.isFinal = false;
            SubtitleRec[textUid] = newSubtitle;

            STTUserOrder.Remove(textUid);
            STTUserOrder.Add(textUid);

            ShowSubtitles();
        }

        void ReceiveFinalText(string finalText, int textUid)
        {
            Debug.LogWarning(string.Format("----------- ReceiveFinalText {0}，from {1}", finalText, textUid));
            ReceiveText(finalText, textUid);
        }

        void ShowSubtitles()
        {
            string text = "";
            foreach (int uid in STTUserOrder)
            {
                var sub = SubtitleRec[uid];
                var color = ColorUtility.ToHtmlStringRGB(SpacePlayer.GetColor((uint)uid));
                text += string.Format("<color=#{2}>[{0}]</color>: {1}\r\n\r\n", uid, sub.Text, color);
            }
            if (SubtitleText != null)
            {
                SubtitleText.text = text;
            }
        }

    }

}

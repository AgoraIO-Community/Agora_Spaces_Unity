// ----------
// STTManager.cs
// 
// Hu Yuhua(darkzero)
// 2023.01.24
// ----------

namespace AgoraSTTSample
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Net;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Google.Protobuf;
    using AgoraSTTSample.Protobuf;
    using AgoraSTTSample.Models;
    using AgoraSTTSample.Utility;

    public class STTManager
    {
        public string appId;

        // public string _sttBaseUrl = "https://api.agora.io/api/";
        public string _sttBaseUrl = "https://stt-demo-staging.agora.io/api/";

        #region - Singleton
        /// <summary>
        /// singleton
        /// </summary>
        private static Lazy<STTManager> _Instance = new Lazy<STTManager>(() => new STTManager());

        private STTManager()
        {
            Console.WriteLine("Created");
        }

        public static STTManager Instance
        {
            get
            {
                return _Instance.Value;
            }
        }
        #endregion

        public STTLangEnum STTLang { get; set; } = STTLangEnum.en_US;

        public async Task<String> Acquire(string channelId)
        {
            string url = String.Format("{0}{1}{2}{3}", this._sttBaseUrl, "v1/projects/", this.appId, "/rtsc/speech-to-text/builderTokens");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            Debug.Log(request.Timeout);

            STTAcquireRequestModel requestModel = new STTAcquireRequestModel();
            requestModel.instanceId = channelId;

            string requestBody = JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Debug.Log(String.Format("Acquire: {0}\nBody: {1}", url, requestBody));

            using (var postStream = new StreamWriter(request.GetRequestStream()))
            {
                postStream.Write(requestBody);
            }

            using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
            {
                //request.EndGetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                STTAcquireResponseModel info = JsonConvert.DeserializeObject<STTAcquireResponseModel>(jsonResponse);
                Debug.Log(info.tokenName);
                return info.tokenName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<string> Start(string sttToken, string channelId, int audioUid, int dataStreamUid)
        {
            string url = String.Format("{0}{1}{2}{3}{4}", this._sttBaseUrl, "v1/projects/", this.appId, "/rtsc/speech-to-text/tasks?builderToken=", sttToken);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            STTStartRequestModel requestModel = new STTStartRequestModel();
            requestModel.audio.agoraRtcConfig.channelName = channelId;
            requestModel.audio.agoraRtcConfig.uid = audioUid.ToString();
            //requestModel.audio.agoraRtcConfig.token = null;

            requestModel.config.recognizeConfig.output.agoraRTCDataStream.channelName = channelId;
            requestModel.config.recognizeConfig.output.agoraRTCDataStream.uid = dataStreamUid.ToString();
            requestModel.config.recognizeConfig.language = STTLangUtil.GetSTTLangString(STTLang);
            //requestModel.config.recognizeConfig.output.agoraRTCDataStream.token = null;

            string requestBody = JsonConvert.SerializeObject(requestModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Debug.Log(String.Format("Start: {0}\nBody: {1}", url, requestBody));

            using (var postStream = new StreamWriter(request.GetRequestStream()))
            {
                postStream.Write(requestBody);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();
                    Debug.Log(String.Format("Start jsonResponse: {0}", jsonResponse));
                    STTStartResponseModel info = JsonConvert.DeserializeObject<STTStartResponseModel>(jsonResponse); //JsonUtility.FromJson<STTStartResponseModel>(jsonResponse); //
                    return info.taskId;
                }
            }
            catch (WebException webError)
            {
                WebResponse res = webError.Response;
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                Debug.Log(string.Format("Start API error: {0}", jsonResponse));
                return "";
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Start other error: {0}", ex.Message));
                return "";
            }
        }

        public async Task<STTQueryResponseModel> Query(string taskId, string sttToken)
        {
            string url = String.Format("{0}{1}{2}{3}{4}{5}{6}", this._sttBaseUrl, "v1/projects/", this.appId, "/rtsc/speech-to-text/tasks/", taskId, "?builderToken=", sttToken);
            Debug.Log(String.Format("Query: {0}", url));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();
                    STTQueryResponseModel info = JsonUtility.FromJson<STTQueryResponseModel>(jsonResponse);
                    return info;
                }
            }
            catch (WebException webError)
            {
                WebResponse res = webError.Response;
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                Debug.Log(string.Format("Query API error: {0}", jsonResponse));
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Query other error: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> Stop(string taskId, string sttToken)
        {
            string url = String.Format("{0}{1}{2}{3}{4}{5}{6}", this._sttBaseUrl, "v1/projects/", this.appId, "/rtsc/speech-to-text/tasks/", taskId, "?builderToken=", sttToken);
            Debug.Log(String.Format("Stop: {0}", url));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 90000;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();
                    STTStopResponseModel info = JsonUtility.FromJson<STTStopResponseModel>(jsonResponse);
                    return true;
                }
            }
            catch (WebException webError)
            {
                WebResponse res = webError.Response;
                StreamReader reader = new StreamReader(res.GetResponseStream());
                string jsonResponse = reader.ReadToEnd();
                Debug.Log(string.Format("Stop API error: {0}", jsonResponse));
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Stop other error: {0}", ex.Message));
                return false;
            }
        }
    }
}
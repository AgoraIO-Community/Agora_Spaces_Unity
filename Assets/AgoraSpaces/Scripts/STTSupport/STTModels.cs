// ----------
// STTModels.cs
// 
// Hu Yuhua(darkzero)
// 2023.01.24
// ----------

namespace AgoraSTTSample.Models
{
    using System;
    using System.Collections.Generic;

    #region - API Models

    #region - Acquire API Model
    /// <summary>
    /// STT Acquire Request Model
    /// </summary>
    class STTAcquireRequestModel
    {
        public string instanceId { get; set; }
    }

    /// <summary>
    /// STT Acquire Response Model
    /// </summary>
    class STTAcquireResponseModel
    {
        public string tokenName { get; set; }
        public string instanceId { get; set; }
        public Nullable<int> createTs { get; set; }
    }
    #endregion

    #region - Start API Model
    class STTStartRequestModel
    {
        public STTStartRequestAudio audio { get; set; } = new STTStartRequestAudio();
        public STTStartRequestConfig config { get; set; } = new STTStartRequestConfig();
    }

    // request body > audio
    class STTStartRequestAudio
    {
        public string subscribeSource { get; set; } = "AGORARTC";
        public STTAgoraRtcConfig agoraRtcConfig { get; set; } = new STTAgoraRtcConfig();
    }

    // request body > audio > agoraRtcConfig
    class STTAgoraRtcConfig
    {
        public string channelName { get; set; }
        public string uid { get; set; }
        public string token { get; set; }
        public string channelType { get; set; } = "LIVE_TYPE";
        public STTSubscribeConfig subscribeConfig { get; set; } = new STTSubscribeConfig();
    }

    // request body > audio > agoraRtcConfig > subscribeConfig
    class STTSubscribeConfig
    {
        public string subscribeMode { get; set; } = "CHANNEL_MODE";
    }

    // request body > config
    class STTStartRequestConfig
    {
        public List<string> features { get; set; } = new List<string>(new string[] { "RECOGNIZE" });
        public STTRecognizeConfig recognizeConfig { get; set; } = new STTRecognizeConfig();
    }

    // request body > config > recognizeConfig
    class STTRecognizeConfig
    {
        //public string language { get; set; } = "zh-CN";
        public string language { get; set; } = "ENG";
        public string model { get; set; } = "Model";
        public bool profanityFilter { get; set; } = true;
        public STTOutput output { get; set; } = new STTOutput();
    }

    // request body > config > recognizeConfig > output
    class STTOutput
    {
        public List<string> destinations { get; set; } = new List<string>(new string[] { "AgoraRTCDataStream" });
        public STTRtcDataStream agoraRTCDataStream { get; set; } = new STTRtcDataStream();
        public STTCloudStorage cloudStorage { get; set; } = null;
    }

    // request body > config > recognizeConfig > output > agoraRTCDataStream
    class STTRtcDataStream
    {
        public string channelName { get; set; }
        public string uid { get; set; }
        public string token { get; set; }
    }

    // request body > config > recognizeConfig > output > cloudStorage
    class STTCloudStorage
    {
        public string format { get; set; } = "HLS";
        public STTCloudStorageConfig storageConfig { get; set; }
    }

    // request body > config > recognizeConfig > output > cloudStorage > storageConfig
    class STTCloudStorageConfig
    {
        public string accessKey { get; set; }
        public string secretKey { get; set; }
        public string bucket { get; set; }
        public string vendor { get; set; }
        public string region { get; set; }
        public List<string> fileNamePrefix { get; set; }
    }

    /// <summary>
    /// STT Start Response Model
    /// </summary>
    public class STTStartResponseModel
    {
        public string taskId { get; set; }
        public string status { get; set; }
    }
    #endregion

    #region - Query API Model
    /// <summary>
    /// STT Query Response Model
    /// </summary>
    public class STTQueryResponseModel
    {
        public string taskId { get; set; }
        public string status { get; set; }
    }
    #endregion

    /// <summary>
    /// STT Stop Response Model
    /// </summary>
    class STTStopResponseModel
    {
        //
    }

    public enum STTLangEnum
    {
        zh_HK,
        zh_CN,
        zh_TW,
        en_IN,
        en_US,
        fr_FR,
        de_DE,
        hi_IN,
        id_ID,
        it_IT,
        ja_JP,
        ko_KR,
        pt_PT,
        es_ES
    }

    public class STTLangUtil
    {
        public static string GetSTTLangString(STTLangEnum langEnum)
        {
            string tempStr = langEnum.ToString();
            return tempStr.Replace('_', '-');
        }
    }
    #endregion
}
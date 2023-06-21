using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Agora.Rtc;
using Agora.Spaces;

public class SpaceMenuCommands : MonoBehaviour
{

    [MenuItem("AgoraSpaces/ClearPrefs")]
    public static void ClearPrefs()
    {
        Debug.LogWarning("Clearing all PlayerPrefs values");
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("AgoraSpaces/InstantiateMe")]
    public static void InstantiateMe()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        go.name = "Cube_" + System.DateTime.Now.ToShortTimeString();

        // configure videoSurface
        VideoSurface vf = go.AddComponent<VideoSurface>();
        vf.SetForUser(0, AgoraSpaceController.Instance.ChannelID);
        vf.SetEnable(true);
    }
}

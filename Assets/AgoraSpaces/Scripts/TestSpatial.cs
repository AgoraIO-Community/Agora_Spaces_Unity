using System.Collections;
using System.Collections.Generic;
using Agora.Rtc;
using UnityEngine;
using UnityEngine.UI;

namespace Agora.Spaces
{
    public class TestSpatial : MonoBehaviour
    {

        [SerializeField]
        uint remoteUid;

        public Slider slider;
        // Start is called before the first frame update
        void Start()
        {
            // Specify a minimum and maximum value for slider.
            slider.maxValue = 10;
            slider.minValue = 0;
            slider.onValueChanged.AddListener(updateSpatialAudioPosition);
        }


        public void updateSpatialAudioPosition(float sourceDistance)
        {
            AgoraSpaceController.Instance.updateSpatialAudioPosition(remoteUid, sourceDistance);
        }

    }
}

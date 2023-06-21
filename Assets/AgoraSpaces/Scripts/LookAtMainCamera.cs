using Mirror;
using UnityEngine;

namespace Agora.Util
{
    public class LookAtMainCamera : MonoBehaviour
    {
        // LateUpdate so that all camera updates are finished.
        [ClientCallback]
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }

    }
}

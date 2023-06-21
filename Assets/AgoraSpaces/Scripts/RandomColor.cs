using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace Agora.Spaces
{
    public class RandomColor : NetworkBehaviour
    {
        // Color32 packs to 4 bytes
        [SyncVar(hook = nameof(SetColor))]
        public Color32 color = Color.black;

        // Unity clones the material when GetComponent<Renderer>().material is called
        // Cache it here and destroy it in OnDestroy to prevent a memory leak
        List<Material> cachedMaterial = new List<Material>();

        [SerializeField]
        List<Renderer> ColorRenders;

        void SetColor(Color32 _, Color32 newColor)
        {
            foreach (var r in ColorRenders)
            {
                if (!cachedMaterial.Contains(r.material))
                {
                    cachedMaterial.Add(r.material);
                }
                r.material.color = newColor;
            }
        }

        void OnDestroy()
        {
            foreach (var m in cachedMaterial)
            {
                if (m != null)
                    Destroy(m);
            }
            cachedMaterial.Clear();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            // This script is on players that are respawned repeatedly
            // so once the color has been set, don't change it.
            if (color == Color.black)
                color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }
}

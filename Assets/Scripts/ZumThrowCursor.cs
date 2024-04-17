using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using zapo;

namespace zum
{
    public class ZumThrowCursor : MonoBehaviour
    {
        public List<ZumMaterial> spheres = new();
        public ZumMaterial arrow;

        public void Awake()
        {
        }
        public void Start()
        {

        }


        public void SetStrengthAndForward(float x, Vector3 forward)
        {
            Quaternion straight = Quaternion.LookRotation(forward, Vector3.up);
            Quaternion tiltUp = Quaternion.Euler(-28, 0, 0);
            this.transform.rotation = straight * tiltUp;
            if (x <= 0 && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            else if (x > 0 && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if (gameObject.activeSelf)
            {
                float computedForward = 0.2f + x * 0.3f;
                for (int i = 0; i < spheres.Count; ++i)
                {
                    spheres[i].gameObject.transform.localPosition = Vector3.forward * computedForward * i / spheres.Count;
                }
                arrow.gameObject.transform.localPosition = Vector3.forward * computedForward;
            }
        }

        public void SetCursorColor(Color c)
        {
            for (int i = 0; i < spheres.Count; ++i)
            {
                spheres[i].DirectApplyColor(c);
            }
            arrow.DirectApplyColor(c);
        }

    }
}
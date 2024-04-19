using System.Collections.Generic;
using UnityEngine;
using zapo;

namespace zum
{
    public class ZumGrabCursor : MonoBehaviour
    {
        public List<ZumMaterial> rings = new();

        [SerializeField]
        private ZapoTimer pulseTimer;

        public void Awake()
        {
        }
        public void Start()
        {

            pulseTimer = new ZapoTimer(1.5f, true, false);
        }

        public void Update()
        {

            if (gameObject.activeSelf)
            {
                pulseTimer.TimerTick(Time.deltaTime);
                float startForward = 0.32f;
                float wiggleBackward = 0.16f;
                for (int i = 0; i < rings.Count; ++i)
                {
                    float ringTarget = (float)i / rings.Count;
                    float zeroToOne = (1.0f + pulseTimer.RemainingPercent() - ringTarget) % 1.0f;
                    float finalAmount = startForward - zeroToOne * wiggleBackward;
                    rings[i].gameObject.transform.localPosition = Vector3.forward * finalAmount;
                    rings[i].gameObject.transform.localScale = Vector3.one * (3.6f - zeroToOne * 1.2f);
                }
            }
        }

        public void SetGrabbing(bool isGrabbing, Vector3 forward)
        {
            if (isGrabbing)
            {
                Quaternion straight = Quaternion.LookRotation(forward, Vector3.up);
                Quaternion tiltUp = Quaternion.Euler(-2, 0, 0);
                this.transform.rotation = straight * tiltUp;
            }
            if (!isGrabbing && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                pulseTimer.Pause();
            }
            else if (isGrabbing && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                pulseTimer.Resume();
            }
        }


        public void SetCursorColor(Color c)
        {
            for (int i = 0; i < rings.Count; ++i)
            {
                float ringTarget = (float)i / rings.Count;
                Color adjusted = Color.Lerp(c, Color.grey, 0.2f + ringTarget * 0.6f);
                rings[i].DirectApplyColor(adjusted);
            }
        }

    }
}
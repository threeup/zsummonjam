using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace zum
{
    public enum HeldStateType
    {
        UNKNOWN,
        DISABLED,
        SLIDING_IN,
        SLIDING_OVER,
        GOOD,
        DISCARDING,
    }
    public class ZumHeldCard : MonoBehaviour
    {
        public Image portrait;
        public Image redBar;
        public Image greenBar;
        public Image blueBar;

        private RectTransform _rt;
        public HeldStateType HeldState = HeldStateType.UNKNOWN;

        public bool IsLeftHand = true;

        public int Slot = -999999;  // -1 is offscreen up in the world, 99 is offscreen down discard

        [SerializeField]
        private Vector3 distToTarget;
        private float posX;
        private float posY;
        [SerializeField]
        private Vector2 target;


        public string minName = "?";

        private float speed = 400.0f;

        public void Awake()
        {
            _rt = GetComponent<RectTransform>();
            posX = -90.0f;
            posY = 100.0f;
            _rt.anchoredPosition = new Vector2(posX, posY);

            AssignValues(new Color(0.6f, 0.1f, 0.99f));
        }

        public void AssignValues(Color c)
        {
            portrait.color = c;
            redBar.rectTransform.sizeDelta = new(Mathf.Round(c.r * 76), 7);
            greenBar.rectTransform.sizeDelta = new(Mathf.Round(c.g * 76), 7);
            blueBar.rectTransform.sizeDelta = new(Mathf.Round(c.b * 76), 7);
        }

        public void Update()
        {
            if (HeldState == HeldStateType.SLIDING_IN || HeldState == HeldStateType.DISCARDING || HeldState == HeldStateType.SLIDING_OVER)
            {
                distToTarget = new Vector3(target.x - posX, target.y - posY, 0);
                Vector3 dir = distToTarget.normalized;
                float appliedSpeed = distToTarget.sqrMagnitude < 20.0f ? speed * 0.5f : speed;
                posX += appliedSpeed * Time.deltaTime * dir.x;
                posY += appliedSpeed * Time.deltaTime * dir.y;
                _rt.anchoredPosition = new Vector2(posX, posY);
            }
            if (HeldState == HeldStateType.SLIDING_IN || HeldState == HeldStateType.SLIDING_OVER)
            {
                if (Vector2.SqrMagnitude(distToTarget) < 6.0f)
                {
                    SetRectPositionToTarget();
                    HeldState = HeldStateType.GOOD;
                }
            }
            if (HeldState == HeldStateType.DISCARDING)
            {
                if (Vector2.SqrMagnitude(distToTarget) < 6.0f)
                {
                    //disable
                    SetSlot(-1, IsLeftHand);
                }
            }
        }

        private void SetRectPositionToTarget()
        {
            posX = target.x;
            posY = target.y;
            _rt.anchoredPosition = new Vector2(posX, posY);
        }

        public void SetSlot(int nextSlot, bool nextLeftHand)
        {
            if (Slot == nextSlot && IsLeftHand == nextLeftHand)
            {
                return;
            }
            Slot = nextSlot;
            IsLeftHand = nextLeftHand;
            if (nextSlot < 0)
            {
                SetRectPositionToTarget();
                HeldState = HeldStateType.DISABLED;
                IsLeftHand = true;
            }
            else if (nextSlot < 10)
            {
                if (IsLeftHand == nextLeftHand)
                {
                    HeldState = HeldStateType.SLIDING_IN;
                }
                else
                {
                    HeldState = HeldStateType.SLIDING_OVER;
                }
            }
            else
            {
                HeldState = HeldStateType.DISCARDING;
            }
            target = ComputeTargetPosition();

            if (HeldState == HeldStateType.DISABLED)
            {
                SetRectPositionToTarget();
                gameObject.SetActive(false);
            }
            else if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        private Vector2 ComputeTargetPosition()
        {
            if (IsLeftHand)
            {
                return Slot switch
                {
                    -1 => new Vector2(-555, 300),
                    0 => new Vector2(-355, 100),
                    1 => new Vector2(-385, 150),
                    2 => new Vector2(-415, 200),
                    3 => new Vector2(-445, 250),
                    _ => new Vector2(-355, -250),
                };
            }
            else
            {
                return Slot switch
                {
                    -1 => new Vector2(555, 300),
                    0 => new Vector2(355, 100),
                    1 => new Vector2(385, 150),
                    2 => new Vector2(415, 200),
                    3 => new Vector2(445, 250),
                    _ => new Vector2(355, -250),
                };
            }
        }
    }
}
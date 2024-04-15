using UnityEngine;
using zapo;

namespace zum
{
    [RequireComponent(typeof(Renderer))]
    public class ZumMaterial : MonoBehaviour
    {
        private Renderer _renderer;
        public bool Randomize = true;

        public float Hue;
        public float Sat;
        public float Lightness;

        [SerializeField]
        private Color _targetColor;

        [SerializeField]
        private Color _adjustedColor;

        public void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        public void Start()
        {
            if (Randomize)
            {
                Hue = Random.Range(0.0f, 1.0f);
                Sat = Random.Range(0.2f, 1.0f);
                Lightness = Random.Range(0.2f, 1.0f);
                _targetColor = ZapoColorHelper.HSLtoRGB(Hue, Sat, Lightness);
                DirectApplyColor(_targetColor);
            }
        }

        public Color GetTargetColorAsRGB()
        {
            return _targetColor;
        }
        public void SetTargetColor(float r, float g, float b)
        {
            _targetColor.r = r;
            _targetColor.g = g;
            _targetColor.b = b;
            BrightenApplyColor(_targetColor);
        }

        public void SetRed(float val)
        {
            _targetColor.r = val;
            BrightenApplyColor(_targetColor);
        }
        public void SetBlue(float val)
        {
            _targetColor.b = val;
            BrightenApplyColor(_targetColor);
        }
        public void SetGreen(float val)
        {
            _targetColor.g = val;
            BrightenApplyColor(_targetColor);
        }
        private void BrightenApplyColor(Color inColor)
        {
            Color.RGBToHSV(inColor, out float hue, out float sat, out float v);
            // bright?
            //_adjustedColor = Color.HSVToRGB(hue, Mathf.Clamp(sat, 0.5f, 1.0f), Mathf.Clamp(v, 0.5f, 1.0f));
            _adjustedColor = Color.HSVToRGB(hue, sat, v);
            _renderer.material.color = _adjustedColor;
        }

        private void DirectApplyColor(Color inColor)
        {
            _adjustedColor = inColor;
            _renderer.material.color = _adjustedColor;
        }
    }
}
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

        private Color _color;

        public void Start()
        {
            _renderer = GetComponent<Renderer>();

            if (Randomize)
            {
                Hue = Random.Range(0.0f, 1.0f);
                Sat = Random.Range(0.7f, 1.0f);
                Lightness = Random.Range(0.7f, 1.0f);
            }

            ApplyColor();
        }

        public Color GetColorAsRGB()
        {
            return _color;
        }

        private void ApplyColor()
        {
            _color = ZapoColorHelper.HSLtoRGB(Hue, Sat, Lightness);
            _renderer.material.color = _color;
        }
    }
}
using UnityEngine;
using zapo;

namespace zum
{
    [RequireComponent(typeof(Renderer))]
    public class ZumMaterial : MonoBehaviour
    {
        private Renderer _renderer;

        public float Hue;
        public float Sat;
        public float Lightness;

        public void Start()
        {
            _renderer = GetComponent<Renderer>();

            Hue = Random.Range(0.0f, 1.0f);
            Sat = Random.Range(0.7f, 1.0f);
            Lightness = Random.Range(0.7f, 1.0f);

            ApplyColor();
        }

        private void ApplyColor()
        {
            _renderer.material.color = ZapoColorHelper.HSLtoRGB(Hue, Sat, Lightness);
        }
    }
}
using UnityEngine;

namespace zapo
{
    public static class ZapoColorHelper
    {

        public static string ColorToHex(Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");

        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static Color AdjustRGBAColor(Color32 color, float r = 1f, float g = 1f, float b = 1f, float a = 1f)
        {
            Color c = color;

            c.r *= r;
            c.g *= g;
            c.b *= b;
            c.a *= a;

            return c;
        }
        public static float HueToRGB(float p, float q, float t)
        {
            if (t < 0f)
                t += 1f;
            if (t > 1f)
                t -= 1f;
            if (t < 1f / 6f)
                return p + (q - p) * 6f * t;
            if (t < 1f / 2f)
                return q;
            if (t < 2f / 3f)
                return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
        // adapted from http://en.wikipedia.org/wiki/HSL_color_space

        public static Color HSLtoRGB(float h, float s, float l)
        {
            Color output = Color.black;
            if (s < 0.01f)
            {
                return output;
            }

            float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
            float p = 2f * l - q;
            output.r = HueToRGB(p, q, h + 1f / 3f);
            output.g = HueToRGB(p, q, h);
            output.b = HueToRGB(p, q, h - 1f / 3f);
            return output;
        }
    }
}
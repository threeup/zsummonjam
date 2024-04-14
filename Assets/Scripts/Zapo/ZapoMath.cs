using UnityEngine;

public static class ZapoMath
{

    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    public static float DistanceToObject(GameObject go, Vector3 pos)
    {
        if (!go)
        {
            return 999.9f;
        }
        Vector3 diff = go.transform.position - pos;
        diff.y = 0.0f;
        return diff.magnitude;
    }

    public static int CarouselIndex(int start, int val, int min, int max)
    {
        int end = start;
        while (Mathf.Abs(val) > 0)
        {
            if (val > 0)
            {
                ++end;
                if (end > max) { end = min; }
                --val;
            }
            else if (val < 0)
            {
                --end;
                if (end < min) { end = max; }
                ++val;
            }
        }
        return end;
    }

    static public int GridDistance(Vector2 a, Vector2 b)
    {
        int xDist = Mathf.Abs(Mathf.RoundToInt(a.x - b.x));
        int yDist = Mathf.Abs(Mathf.RoundToInt(a.y - b.y));
        return Mathf.Max(xDist, yDist);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillUtil
{
    public static bool IsRectangleCollide(Rect rect1, Rect rect2)
    {
        return IsRectangleCollide(rect1.center.x, rect1.center.y, rect1.width, rect1.height, rect2.center.x, rect2.center.y, rect2.width, rect2.height);
    }

    public static bool IsRectangleCollide(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
    {
        bool xCheck = Mathf.Abs(x1 - x2) <= (w1 + w2) / 2;
        bool yCheck = Mathf.Abs(y1 - y2) <= (h1 + h2) / 2;
        return xCheck && yCheck;
    }
}

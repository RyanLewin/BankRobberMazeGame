using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vec2
{
    //Simple Vector2
    public int x, y;
    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    //Simple operation to add 2 2D vectors
    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }
}

using UnityEngine;
using System.Collections;

//All the directions
public enum MapDirection
{
    North,
    East,
    South,
    West
}

public static class MapDirections {

    public const int count = 4;
    
    //2D vectors of dir
    private static Vec2[] vectors =
    {
        new Vec2(0,1),
        new Vec2(1,0),
        new Vec2(0,-1),
        new Vec2(-1,0)
    };

    //3D vectors of dir
    private static Vector3[] vectors3 =
    {
        new Vector3(0,0,1f),
        new Vector3(1f,0,0),
        new Vector3(0,0,-1f),
        new Vector3(-1f,0,0)
    };
    
    //Opposite of each vector
    private static MapDirection[] opposites =
    {
        MapDirection.South,
        MapDirection.West,
        MapDirection.North,
        MapDirection.East
    };

    //the rotations for each direction
    private static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    public static Quaternion ToRotation (this MapDirection dir)
    {
        return rotations[(int)dir];
    }

    public static MapDirection GetOpposite (this MapDirection dir)
    {
        return opposites[(int)dir];
    }

    public static Vec2 ToIntVec2 (this MapDirection dir)
    {
        return vectors[(int)dir];
    }

    public static Vector3 ToVec3 (this MapDirection dir)
    {
        return vectors3[(int)dir];
    }

    public static MapDirection RandomValue
    {
        get { return (MapDirection)Random.Range(0, count); }
    }
}

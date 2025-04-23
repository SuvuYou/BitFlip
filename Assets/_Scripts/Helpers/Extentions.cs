using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Right, Down, Left, None }

public static class DirectionExtentions
{
    public static List<Direction> AllDirections = new () { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

    public static Direction Opposite(this Direction direction) =>
        direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.None
        };

    public static Direction LocalRight(this Direction direction) =>
        direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.None => Direction.Right,
            _ => Direction.None
        };

    public static Direction LocalLeft(this Direction direction) =>
        direction switch
        {
            Direction.Up => Direction.Left, 
            Direction.Right => Direction.Up,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Down,
            Direction.None => Direction.Left,
            _ => Direction.None
        };

        public static Direction LocalForward(this Direction direction) =>
        direction switch
        {
            Direction.Up => Direction.Up, 
            Direction.Right => Direction.Right,
            Direction.Down => Direction.Down,
            Direction.Left => Direction.Left,
            Direction.None => Direction.Up,
            _ => Direction.None
        };

    public static Vector2Int ToVector(this Direction direction) =>
        direction switch
        {
            Direction.Up => new Vector2Int(0, 1), 
            Direction.Right => new Vector2Int(1, 0),
            Direction.Down => new Vector2Int(0, -1),
            Direction.Left => new Vector2Int(-1, 0),
            Direction.None => new Vector2Int(0, 0),
            _ => new Vector2Int(0, 0)
        };
}

public static class TransformExtentions
{
    public static bool TryGetComponentInChildren<T>(this Transform transform, out T component) where T : Component
    {
        component = transform.GetComponentInChildren<T>();

        return component != null;
    }

    public static bool TryGetComponentInChildrenOfParent<T>(this Transform transform, out T component) where T : Component
    {
        var parent = transform.parent;

        if (parent == null) parent = transform;

        component = parent.GetComponentInChildren<T>();

        return component != null;
    }
}

public static class VectorExtentions
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        vector.x = x ?? vector.x;
        vector.y = y ?? vector.y;
        vector.z = z ?? vector.z;

        return vector;
    }

    public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
    {
        vector.x = x ?? vector.x;
        vector.y = y ?? vector.y;

        return vector;
    }

    public static Vector3 ToVector3WithZ(this Vector2 vector, float z) => new (vector.x, vector.y, z);

    public static Vector3Int ToVector3WithZ(this Vector2Int vector, int z) => new (vector.x, vector.y, z);
} 

public static class FloatExtentions
{
    public static float AbsoluteValue(this float value) => Mathf.Abs(value);
}

public static class AnimatorExtentions
{
    public static float GetClipLength(this Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        
        return 0f;
    }
}

public static class GameObjectExtentions
{
    public static bool IsPrefab(this GameObject gameObject) => !gameObject.scene.IsValid();
}
using UnityEngine;
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
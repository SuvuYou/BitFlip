using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Vector3UnityEvent : UnityEvent<Vector3> { }

[System.Serializable]
public class Vector3QuaternionUnityEvent : UnityEvent<Vector3, Quaternion> { }

[System.Serializable]
public class FloatUnityEvent : UnityEvent<float> { }

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CollisionContextDataSO")]
public class CollisionContextDataSO : ContextDataSO, ICollisionContextData
{
    public Action<Collider2D> OnTriggerEnter { get; set; }
    public Action<Collision2D> OnCollisionEnter { get; set; }
}
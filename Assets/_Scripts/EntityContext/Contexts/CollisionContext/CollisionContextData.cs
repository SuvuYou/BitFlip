using System;
using UnityEngine;

public interface ICollisionContextData : IContextData
{
    public Action<Collider2D> OnTriggerEnter { get; set; }
    public Action<Collision2D> OnCollisionEnter { get; set; }
    public Action<RaycastHit2D, Direction> OnRaycastHitWall { get; set; }
}

public class CollisionContextData : ICollisionContextData
{
    public Action OnContextInjected { get; set; }

    public Action<Collider2D> OnTriggerEnter { get; set; }
    public Action<Collision2D> OnCollisionEnter { get; set; }
    public Action<RaycastHit2D, Direction> OnRaycastHitWall { get; set; }
}
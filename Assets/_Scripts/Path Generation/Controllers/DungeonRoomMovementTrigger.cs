using System;
using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomMovementTrigger : MonoBehaviour
    {
        public static DungeonRoomMovementTrigger CreateInstanceAt(int x, int y)
        {
            GameObject go = new ("DungeonRoomMovementTrigger");
            go.transform.position = new Vector3(x, y, 0);

            DungeonRoomMovementTrigger trigger = go.AddComponent<DungeonRoomMovementTrigger>();

            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f);

            return trigger;
        }

        public event Action OnMovementDetected;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.transform.TryGetComponentInChildrenOfParent<PlayerContextProvider>(out _)) return;

            OnMovementDetected?.Invoke();

            Destroy(gameObject);
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        } 
    }
}

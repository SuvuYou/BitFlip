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
            trigger.InitPosition(x, y);

            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f);

            return trigger;
        }

        private int _x , _y ;
        public void InitPosition(int x, int y) => (_x, _y) = (x, y);
        
        public event Action<Vector2Int> OnMovementDetected;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.transform.TryGetComponentInChildrenOfParent<PlayerContextProvider>(out _)) return;

            OnMovementDetected?.Invoke(new Vector2Int(_x, _y));

            Destroy(gameObject);
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        } 
    }
}

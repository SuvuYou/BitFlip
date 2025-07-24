using System;
using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomEnterringTrigger : MonoBehaviour
    {
        public static DungeonRoomEnterringTrigger CreateInstanceAt(int x, int y)
        {
            GameObject go = new ("DungeonRoomEnterringTrigger");
            go.transform.position = new Vector3(x, y, 0);

            DungeonRoomEnterringTrigger trigger = go.AddComponent<DungeonRoomEnterringTrigger>();

            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f);

            return trigger;
        }

        public event Action OnRoomEntered;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.transform.TryGetComponentInChildrenOfParent<PlayerContextProvider>(out _)) return;

            OnRoomEntered?.Invoke();

            Destroy(gameObject);
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        } 
    }
}

using System.Collections;
using System.Threading.Tasks;
using PathGeneration;
using UnityEngine;

namespace MapRendering
{
    public class DungeonDoorController : MonoBehaviour
    {
        [SerializeField] private DungeonDoorView _view;

        public void Init(DungeonDoorData doorData)
        {
            _view.Init(doorData.Position.Global);
            // _view.SetDirection(doorData.LeadingDirection);
            // _view.SetState(doorData.State);
        }

        public Task Open(DungeonDoorData doorData)
        {
            doorData.SetState(DungeonDoorState.Open);

            return _view.PlayOpenAnimation();
        }

        public void Lock(DungeonDoorData doorData)
        {
            doorData.SetState(DungeonDoorState.Locked);
            _view.PlayLockAnimation();
        }
    }
}

using UnityEngine;

public class CollisionContextProviderSO : ContextProvider<ICollisionContextData> 
{
    [SerializeField] private CollisionContextDataSO _collisionContextDataSO;

    protected override void Awake() 
    {
        _contextData = _collisionContextDataSO;

        base.Awake();
    }
}

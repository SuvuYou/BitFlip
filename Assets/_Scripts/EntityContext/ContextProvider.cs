using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ContextProvider<TContext> : MonoBehaviour where TContext : IContextData
{
    [SerializeField] protected TContext _contextData;

    protected virtual void Awake()
    {
        var consumers = GetComponentsInChildren<IConsumer<TContext>>();

        foreach (var c in consumers) c.Inject(_contextData);
    }
}
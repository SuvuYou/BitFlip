using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ContextProvider<TContext> : MonoBehaviour where TContext : IContextData
{
    protected TContext _contextData;

    protected virtual void Awake()
    {
        var consumers = GetComponentsInChildren<IConsumer<TContext>>();

        foreach (var c in consumers) c.Inject(_contextData);

        Debug.Log( _contextData.OnContextInjected + "  " + this);

        _contextData.OnContextInjected?.Invoke();
    }
}

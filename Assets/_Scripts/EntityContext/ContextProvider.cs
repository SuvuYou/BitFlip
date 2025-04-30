using UnityEngine;

public class ContextProvider<TContext> : MonoBehaviour where TContext : IContextData
{
    [SerializeField] private TContext _contextData;

    private void Awake()
    {
        var consumers = GetComponentsInChildren<IConsumer<TContext>>();

        foreach (var c in consumers) c.Inject(_contextData);
    }
}
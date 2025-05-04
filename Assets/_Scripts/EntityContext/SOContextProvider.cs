using UnityEngine;

public class SOContextProvider<T> : MonoBehaviour where T : ContextDataSO
{
    [SerializeField] private T context;

    void Awake()
    {
        var consumers = GetComponentsInChildren<IConsumer<T>>();
        
        foreach (var c in consumers) c.Inject(context);
    }
}
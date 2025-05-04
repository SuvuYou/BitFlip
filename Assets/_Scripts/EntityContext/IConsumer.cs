using UnityEngine;

public interface IConsumer<TContext>
{
    TContext Context { get; }

    void Inject(TContext context);
}

public abstract class ConsumerBase<TContext> : MonoBehaviour, IConsumer<TContext>
{
    public TContext Context { get; private set; }

    public virtual void Inject(TContext context)
    {
        Context = context;
        OnInjected();
    }

    protected virtual void OnInjected() { }
}
public interface IHealthContextData : IContextData
{
    public EntityHealthState HealthState { get; }

    public void SetHealthState(EntityHealthState state);
}

public class HealthContextData : IHealthContextData
{
    public EntityHealthState HealthState { get; private set; }

    public void SetHealthState(EntityHealthState state) => HealthState = state;

    public HealthContextData() 
    {
        HealthState = new EntityHealthState();        
    }
}

public class HealthContextProvider : ContextProvider<IHealthContextData>
{
    protected override void Awake() 
    {
        _contextData = new HealthContextData();

        base.Awake();
    }
}
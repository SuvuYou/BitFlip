public class Timer
{
    public Timer(float time = 0f)
    {
        _defaultTime = time;
    }

    private float _defaultTime;
    public float Time { get; private set; }
    public bool IsRunning { get; private set; }

    public float Duration => _defaultTime;   

    public bool IsFinished => Time <= 0f;   

    public float Percentage => Time / _defaultTime;
    public float CompletenessPercentage => 1 - Percentage;

    public void Update(float deltaTime) => Time -= IsRunning ? deltaTime : 0;

    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;

    public void Reset() => Time = _defaultTime;
    public void SetBaseTime(float time) 
    {
        _defaultTime = time; 
        Reset();
    }
}
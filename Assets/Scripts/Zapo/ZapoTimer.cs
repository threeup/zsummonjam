
using UnityEngine;
[System.Serializable]
public class ZapoTimer
{
    public float CountdownTime { get; set; }
    [field: SerializeField] public bool IsLooping { get; set; }

    [field: SerializeField] public bool IsCountingDown { get; private set; }

    [SerializeField] private float _countdownTimer;

    public ZapoTimer(float time, bool looping, bool countingDown)
    {
        CountdownTime = time;
        IsLooping = looping;
        IsCountingDown = countingDown;
    }
    public void Launch()
    {
        Reset();
        IsCountingDown = true;
    }
    public void Resume()
    {
        IsCountingDown = true;
    }
    public void Reset()
    {
        _countdownTimer = CountdownTime;
    }
    public void Stop()
    {
        _countdownTimer = -1.0f;
        IsCountingDown = false;
    }

    public float RemainingPercent()
    {
        return 1 - _countdownTimer / CountdownTime;
    }

    public bool TimerTick(float dt)
    {
        if (!IsCountingDown)
        {
            return false;
        }
        if (_countdownTimer < 0.0f)
        {
            return false;
        }
        _countdownTimer -= dt;
        if (_countdownTimer < 0.0f)
        {
            if (IsLooping)
            {
                Launch();
            }
            else
            {
                Stop();
            }
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return _countdownTimer.ToString("n2") + "/" + CountdownTime.ToString("n2");
    }
}
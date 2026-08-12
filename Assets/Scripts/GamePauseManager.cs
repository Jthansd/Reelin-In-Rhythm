using System;
using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    private int pauseRequests = 0;
    private float previousTimeScale = 1f;

    public bool IsPaused => pauseRequests > 0;

    public event Action<bool> OnPauseStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestPause()
    {
        if (pauseRequests == 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;
            OnPauseStateChanged?.Invoke(true);
        }
        pauseRequests++;
    }

    public void ReleasePause()
    {
        if (pauseRequests <= 0) return; // guard against a stray release with no matching request

        pauseRequests--;
        if (pauseRequests == 0)
        {
            Time.timeScale = previousTimeScale;
            AudioListener.pause = false;
            OnPauseStateChanged?.Invoke(false);
        }
    }
}
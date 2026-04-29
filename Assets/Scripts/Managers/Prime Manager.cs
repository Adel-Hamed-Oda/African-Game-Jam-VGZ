using System;
using UnityEngine;

public class PrimeManager : SingletonBehaviour<PrimeManager>
{
    public Action OnGamePaused;
    public Action OnGameResumed;

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }
}

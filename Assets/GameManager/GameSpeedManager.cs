using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEditor;

public enum PauseToken {
  NONE = 0,
  MESSAGE_BOX = 1,
  SETTINGS = 2,
  END = 3,
}

public class GameSpeedManager : MonoBehaviour {
  public static GameSpeedManager Instance;
  public static event Action<float> OnTimeScaleChanged = delegate { };

  private Dictionary<PauseToken, bool> pauseState = new();
  private float normalSpeed = 1.0f;
  private float turboBoostTime = 4.0f;
  private float previousSpeed = 0.0f;

  private void Awake() {
    Instance = this;
    pauseState[PauseToken.NONE] = false;
    Time.timeScale = normalSpeed;
    previousSpeed = normalSpeed;
  }

  public bool IsPaused() {
    return pauseState.Any(kvp => kvp.Value);
  }

  public void HandlePause(PauseToken token = PauseToken.NONE) {
    if (!pauseState.ContainsKey(token)) pauseState.Add(token, true);
    else pauseState[token] = !pauseState[token];
    UpdateTimes(token == PauseToken.NONE);
  }

  public void Pause(PauseToken token = PauseToken.NONE) {
    if (!pauseState.ContainsKey(token)) pauseState.Add(token, true);
    else pauseState[token] = true;
    UpdateTimes(token == PauseToken.NONE);
  }

  public void Unpause(PauseToken token = PauseToken.NONE) {
    if (!pauseState.ContainsKey(token)) pauseState.Add(token, false);
    else pauseState[token] = false;
    UpdateTimes(token == PauseToken.NONE);
  }

  private void UpdateTimes(bool shouldNotifyChange) {
    if (Time.timeScale != 0) {
      previousSpeed = Time.timeScale;
    }
    if (IsPaused()) {
      Time.timeScale = 0;
      if (shouldNotifyChange) {
        OnTimeScaleChanged.Invoke(Time.timeScale);
      }
    }  else {
      Time.timeScale = previousSpeed;
      if (shouldNotifyChange) {
        OnTimeScaleChanged.Invoke(Time.timeScale);
      }
    }
  }

  public void ToggleTurboBoost() {
    if (Time.timeScale == 0) return;
    if (Time.timeScale == turboBoostTime) {
      SetGameSpeed(normalSpeed);
    } else {
      SetGameSpeed(turboBoostTime);
    }
  }
  
  public void SetGameSpeed(float speedFactor) {
    if (speedFactor == 0)  Pause();
    if (Time.timeScale == 0 && speedFactor > 0) Unpause();
    if (IsPaused()) {
      previousSpeed = normalSpeed * speedFactor;
    } else {
      Time.timeScale = normalSpeed * speedFactor;

    }
  }
}

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

  public static event Action<bool> OnPauseChanged = delegate { };
  private Dictionary<PauseToken, bool> pauseState = new();
  private float normalSpeed = 1.0f;
  private float turboBoostTime = 4.0f;

  private void Awake() {
    Instance = this;
    pauseState[PauseToken.NONE] = false;
    Time.timeScale = normalSpeed;
  }

  private bool IsPaused() {
    return pauseState.Any(kvp => kvp.Value);
  }

  public void HandlePause(PauseToken token = PauseToken.NONE) {
    if (!pauseState.ContainsKey(token)) pauseState.Add(token, true);
    else pauseState[token] = !pauseState[token];

    OnPauseChanged?.Invoke(pauseState[PauseToken.NONE]);
    Time.timeScale = IsPaused() ? 0 : normalSpeed;
  }

  public void ToggleTurboBoost() {
    if (Time.timeScale == 0) return;
    if (Time.timeScale == turboBoostTime) {
      Time.timeScale = normalSpeed;
    } else {
      Time.timeScale = turboBoostTime;
    }
  }
}

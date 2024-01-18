using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public enum PauseToken {
  NONE = 0,
  MESSAGE_BOX = 1,
  SETTINGS = 2,
  END = 3,
}

public class PauseManager : MonoBehaviour {
  public static PauseManager Instance;

  public static event Action<bool> OnPauseChanged = delegate { };
  private bool paused;
  private bool screenPaused;
  private Dictionary<PauseToken, bool> pauseState = new();


  private void Awake() {
    Instance = this;
  }

  private bool IsPaused() {
    return pauseState.Any(kvp => kvp.Value) || paused;
  }

  //TODO nnewsom have pause take a token.
  public void HandlePause(PauseToken token = PauseToken.NONE) {
    if (token == PauseToken.NONE) {
      paused = !paused;
      OnPauseChanged?.Invoke(paused);
    } else {
      if (!pauseState.ContainsKey(token)) pauseState.Add(token, true);
      else pauseState[token] = !pauseState[token];
    }
    
    Time.timeScale = IsPaused() ? 0 : 1;
  }
}

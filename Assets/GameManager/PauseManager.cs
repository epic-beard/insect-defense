using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour {
  public static PauseManager Instance;

  public static event Action<bool> OnPauseChanged = delegate { };
  private bool paused;
  private bool screenPaused;

  private void Awake() {
    Instance = this;
  }

  //TODO nnewsom have pause take a token.
  public void HandlePause() {
    paused = !paused;
    Time.timeScale = paused || screenPaused ? 0 : 1;
    OnPauseChanged?.Invoke(paused);
  }

  public void HandleScreenPause() {
    screenPaused = !screenPaused;
    Time.timeScale = paused || screenPaused ? 0 : 1;
  }
}

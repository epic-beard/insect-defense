using UnityEngine;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour {
  public static PauseManager Instance;

  private bool paused;
  private bool screenPaused;

  private void Awake() {
    Instance = this;
  }

  public void HandlePause() {
    paused = !paused;
    Time.timeScale = paused || screenPaused ? 0 : 1;
    TerrariumBottomUI.Instance.KeepPlayPauseButtonNameCorrect();
  }

  public void HandleScreenPause() {
    screenPaused = !screenPaused;
    Time.timeScale = paused || screenPaused ? 0 : 1;
  }
}

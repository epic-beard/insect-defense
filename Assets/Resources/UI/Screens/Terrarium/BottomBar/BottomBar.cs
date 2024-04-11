using UnityEngine;
using UnityEngine.UIElements;

public class BottomBar : MonoBehaviour {
  readonly private string playPauseButtonName = "play-pause-button";
  readonly private string settingsButtonName = "settings-button";

  readonly private string playString = "Play";
  readonly private string pauseString = "Pause";

  private UIDocument uiDocument;
  private Button playPauseButtonVE;
  private Button settingsButtonVE;

  private void Awake() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    playPauseButtonVE = rootElement.Q<Button>(playPauseButtonName);
    settingsButtonVE = rootElement.Q<Button>(settingsButtonName);
    GameSpeedManager.OnPauseChanged += KeepPlayPauseButtonNameCorrect;
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    playPauseButtonVE.RegisterCallback<ClickEvent>(
        (ClickEvent) => { GameSpeedManager.Instance.HandlePause(); });
    settingsButtonVE.RegisterCallback<ClickEvent>(
        (ClickEvent) => { TerrariumInputManager.Instance.ToggleSettings(); });
  }

  public void KeepPlayPauseButtonNameCorrect(bool paused) {
    if (paused) {
      playPauseButtonVE.text = playString;
    } else {
      playPauseButtonVE.text = pauseString;
    }
  }
}

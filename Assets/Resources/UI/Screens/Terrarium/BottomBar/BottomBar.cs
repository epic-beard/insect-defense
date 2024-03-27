using UnityEngine;
using UnityEngine.UIElements;

public class BottomBar : MonoBehaviour {
  readonly private string nuLabelName = "nu-amount-label";
  readonly private string playPauseButtonName = "play-pause-button";
  readonly private string settingsButtonName = "settings-button";

  readonly private string playString = "Play";
  readonly private string pauseString = "Pause";

  private UIDocument uiDocument;
  private Button playPauseButtonVE;
  private Button settingsButtonVE;
  private Label nuLabelVE;

  private void Awake() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    playPauseButtonVE = rootElement.Q<Button>(playPauseButtonName);
    settingsButtonVE = rootElement.Q<Button>(settingsButtonName);
    nuLabelVE = rootElement.Q<Label>(nuLabelName);
    GameStateManager.OnNuChanged += UpdateNu;
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

  public void UpdateNu(int nu) {
    nuLabelVE.text = nu.ToString();
  }
}

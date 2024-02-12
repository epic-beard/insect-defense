using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumBottomUI : MonoBehaviour {
  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";
  readonly private string nuLabelName = "nu_amount__label";

  private UIDocument terrariumScreen;
  private Button playPauseButton;
  private Button settingsButton;
  private Label nuLabel;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
    nuLabel = rootElement.Q<Label>(nuLabelName);
    GameStateManager.OnNuChanged += UpdateNu;
    PauseManager.OnPauseChanged += KeepPlayPauseButtonNameCorrect;
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(
        (ClickEvent) => { PauseManager.Instance.HandlePause(); });
    settingsButton.RegisterCallback<ClickEvent>(
        (ClickEvent) => { TerrariumInputManager.Instance.ToggleSettings(); });
  }

  public void KeepPlayPauseButtonNameCorrect(bool paused) {
    if (paused) {
      playPauseButton.text = "Play";
    } else {
      playPauseButton.text = "Pause";
    }
  }

  public void UpdateNu(int nu) {
    nuLabel.text = nu.ToString();
  }
}

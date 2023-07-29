using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumBottomUI : MonoBehaviour {
  public static TerrariumBottomUI Instance;

  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";

  private UIDocument terrariumScreen;
  private Button playPauseButton;
  private Button settingsButton;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);

    Instance = this;
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(
      (ClickEvent) => { PauseManager.Instance.HandlePause(); });
    settingsButton.RegisterCallback<ClickEvent>(
    (ClickEvent) => { SettingsScreen.Instance.ToggleSettings(); });
  }

  public void KeepPlayPauseButtonNameCorrect() {
    if (Time.timeScale == 0) {
      playPauseButton.text = "Play";
    } else {
      playPauseButton.text = "Pause";
    }
  }
}

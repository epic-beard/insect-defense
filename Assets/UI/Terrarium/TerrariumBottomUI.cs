using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumBottomUI : MonoBehaviour {

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
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(PauseManager.Instance.HandlePauseCallback);
    playPauseButton.RegisterCallback<ClickEvent>(KeepPlayPauseButtonNameCorrect);
    settingsButton.RegisterCallback<ClickEvent>(SettingsMenu.Instance.ToggleSettingsCallback);
  }

  private void KeepPlayPauseButtonNameCorrect(ClickEvent evt) {
    if (Time.timeScale == 0) {
      playPauseButton.text = "Play";
    } else {
      playPauseButton.text = "Pause";
    }
  }
}

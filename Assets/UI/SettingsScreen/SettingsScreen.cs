using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour {
  public static SettingsScreen Instance;

  private UIDocument settingsScreen;

  private void Awake() {
    Instance = this;
    SetVisualElements();
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    settingsScreen = GetComponent<UIDocument>();
  }

  public void ToggleSettings() {
    if (settingsScreen.rootVisualElement.style.display == DisplayStyle.None) {
      OpenSettings();
    } else {
      CloseSettings();
    }
  }

  public void OpenSettings() {
    PauseManager.Instance.HandleScreenPause();
    InputManager.Input.SwitchCurrentActionMap("SettingsScreen");
    settingsScreen.rootVisualElement.style.display = DisplayStyle.Flex;
    TerrariumUI.Instance.HideUI();
  }

  public void CloseSettings() {
    PauseManager.Instance.HandleScreenPause();
    InputManager.SwitchToSceneActionMap();
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
    TerrariumUI.Instance.ShowUI();
  }
}

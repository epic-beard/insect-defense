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
      InputManager.Instance.OpenSettings();
    } else {
      InputManager.Instance.CloseSettings();
    }
  }

  public void ShowSettings() {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  public void HideSettings() {
    settingsScreen.rootVisualElement.style.display = DisplayStyle.None;
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour {
  UIDocument settingsMenu;

  private void OnEnable() {
    SetVisualElements();
  }
  private void Update() {
    if (Input.GetKeyDown(KeyCode.S)) {
      ToggleSettings();
    }
  }

  private void ToggleSettings() {
    if (!settingsMenu.enabled) {
      OpenSettings();
    } else {
      CloseSettings();
    }
  }

  private void SetVisualElements() {
    settingsMenu = GetComponent<UIDocument>();
  }

  private void OpenSettings() {
    PauseManager.Instance.PauseAndLock();
    settingsMenu.enabled = true;
  }

  private void CloseSettings() {
    PauseManager.Instance.UnpauseAndUnlock();
    settingsMenu.enabled = false;
  }
}

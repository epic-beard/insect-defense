using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour {
  public static SettingsMenu Instance;

  private UIDocument settingsMenu;

  private void Awake() {
    Instance = this;
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

  public void ToggleSettingsCallback(ClickEvent evt) {
    ToggleSettings();
  }

  public static void ShenanigansCallback(ClickEvent evt) {
    Instance.ToggleSettings();
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

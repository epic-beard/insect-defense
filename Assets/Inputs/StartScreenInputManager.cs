using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreenInputManager : MonoBehaviour {
  public static StartScreenInputManager Instance;

  private StartScreenInputs actions;
  private InputActionMap? oldMap;
  private InputActionMap currMap;

  private void Awake() {
    Instance = this;
    actions = new();
  }

  private void OnEnable() {
    actions.StartScreen.Enable();
    currMap = actions.StartScreen;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  public void OpenSettings() {
    UpdateActions(actions.SettingsScreen);
    SettingsScreen.Instance.OpenSettings();
  }

  public void OpenLoadScreen() {
    OpenSettings();
    SettingsScreen.Instance.OpenLoadOptions();
  }

  private void OnCloseSettings(InputAction.CallbackContext context) {
    SettingsScreen.Instance.CloseSettings();
    StartScreen.Instance.ShowStartScreen();
    UpdateActions(oldMap);
  }

  public void OnDisable() {
    actions.Disable();
  }

  private void UpdateActions(InputActionMap map) {
    oldMap = currMap;
    oldMap?.Disable();

    currMap = map;
    currMap.Enable();
  }
}

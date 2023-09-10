using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreenInputManager : InputManager {
  private void Awake() {
    Instance = this;
    input = GetComponent<PlayerInput>();
  }

  private void OnEnable() {
    input.SwitchCurrentActionMap("StartScreen");

    // [TODO] input keyboard navigation of load screen.
    //Input.actions["LoadScreen_Navigate"].started += LoadScreenNavigate;
    //Input.actions["LoadScreen_Select"].started += LoadScreenSelect;
    input.actions["LoadScreen_Close"].started += ExitLoadScreen;

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    input.actions["SettingsScreen_Close"].started += ExitSettingsScreen;
  }

  private void ExitSettingsScreen(InputAction.CallbackContext context) {
    CloseSettings();
    StartScreen.Instance.ShowStartScreen();
    SwitchToActionMap("StartScreen");
  }

  private void ExitLoadScreen(InputAction.CallbackContext context) {
    LoadScreen.Instance.CloseMenu();
    StartScreen.Instance.ShowStartScreen();
    SwitchToActionMap("StartScreen");
  }
}

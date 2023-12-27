using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreenInputManager : MonoBehaviour {
  StartScreenInputs actions;
  public static StartScreenInputManager Instance;
  private void Awake() {
    Instance = this;
    actions = new();
  }

  private void OnEnable() {
    actions.StartScreen.Enable();

    // [TODO] input keyboard navigation of load screen.
    //Input.actions["LoadScreen_Navigate"].started += LoadScreenNavigate;
    //Input.actions["LoadScreen_Select"].started += LoadScreenSelect;
    actions.LoadScreen.LoadScreen_Close.started += OnCloseLoadScreen;

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  public void OpenSettings() {
    actions.StartScreen.Disable();
    actions.SettingsScreen.Enable();
    SettingsScreen.Instance.ShowSettings();
  }

  private void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    SettingsScreen.Instance.HideSettings();
    StartScreen.Instance.ShowStartScreen();
    actions.SettingsScreen.Disable();
    actions.StartScreen.Enable();
  }

  public void OpenLoadScreen() {
    actions.StartScreen.Disable();
    actions.LoadScreen.Enable();
  }
  private void OnCloseLoadScreen(InputAction.CallbackContext context) {
    CloseLoadScreen();
  }
  public void CloseLoadScreen() {
    LoadScreen.Instance.CloseMenu();
    StartScreen.Instance.ShowStartScreen();
    actions.LoadScreen.Disable();
    actions.StartScreen.Enable();
  }
}

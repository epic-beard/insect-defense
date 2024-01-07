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

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  public void OpenSettings() {
    actions.StartScreen.Disable();
    actions.SettingsScreen.Enable();
    SettingsScreen.Instance.OpenSettings();
  }

  public void OpenLoadScreen() {
    actions.StartScreen.Disable();
    actions.SettingsScreen.Enable();
    SettingsScreen.Instance.OpenSettings();
    SettingsScreen.Instance.OpenLoadOptions();
  }

  private void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    SettingsScreen.Instance.CloseSettings();
    StartScreen.Instance.ShowStartScreen();
    actions.SettingsScreen.Disable();
    actions.StartScreen.Enable();
  }
}

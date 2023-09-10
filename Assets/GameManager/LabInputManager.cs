using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class LabInputManager : InputManager {
  private void Awake() {
    Instance = this;
    input = GetComponent<PlayerInput>();
  }

  private void OnEnable() {
    input.SwitchCurrentActionMap("Lab");

    // [TODO] input keyboard navigation of load screen.
    //Input.actions["LoadScreen_Navigate"].started += LoadScreenNavigate;
    //Input.actions["LoadScreen_Select"].started += LoadScreenSelect;
    input.actions["LoadScreen_Close"].started += ExitLoadScreen;

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    input.actions["SettingsScreen_Close"].started += ExitSettingsScreen;

    input.actions["Lab_Load"].started += EnterLoadScreen;
    input.actions["Lab_Settings"].started += EnterSettingsScreen;

    input.actions["Selected_Back"].started += ReturnCamera;
  }

  private void EnterSettingsScreen(InputAction.CallbackContext context) {
    SettingsScreen.Instance.OpenSettings();
  }

  private void ExitSettingsScreen(InputAction.CallbackContext context) {
    SettingsScreen.Instance.CloseSettings();
  }

  private void EnterLoadScreen(InputAction.CallbackContext context) {
    input.SwitchCurrentActionMap("LoadScreen");
    LoadScreen.Instance.OpenMenu();
  }

  private void ExitLoadScreen(InputAction.CallbackContext context) {
    LoadScreen.Instance.CloseMenu();
    SwitchToActionMap("Lab");
  }

  private void ReturnCamera(InputAction.CallbackContext context) {
    LabCamera.Instance.ReturnCamera();
    SwitchToActionMap("Lab");
  }
}

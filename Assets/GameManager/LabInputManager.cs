using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class LabInputManager : MonoBehaviour {
  private LabInputs actions;
  public static LabInputManager Instance;

  private void Awake() {
    Instance = this;
    actions = new();
  }

  private void OnEnable() {
    actions.Lab.Enable();

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;

    actions.Lab.Lab_Settings.started += OnOpenSettings;

    actions.Selected.Selected_Back.started += OnReturnCamera;
    actions.Terrarium.Terrarium_Back.started += OnCloseTerrarium;
  }

  private void OnOpenSettings(InputAction.CallbackContext context) {
    OpenSettings();
  }
  public void OpenSettings() {
    actions.Lab.Disable();
    actions.SettingsScreen.Enable();
    SettingsScreen.Instance.OpenSettings();
  }

  private void OnCloseSettings(InputAction.CallbackContext context) {
    SettingsScreen.Instance.CloseSettings();
    actions.SettingsScreen.Disable();
    actions.Lab.Enable();
  }

  private void OnReturnCamera(InputAction.CallbackContext context) {
    LabCamera.Instance.ReturnCamera();
    actions.Selected.Disable();
    actions.Lab.Enable();
  }

  private void OnCloseTerrarium(InputAction.CallbackContext context) {
    Terrarium.Selected?.CloseScreen();
  }

  public void EnableSelectedActionMap() {
    actions.Lab.Disable();
    actions.Selected.Enable();
  }
  public void EnableTerrariumActionMap() {
    actions.Lab.Disable();
    actions.Terrarium.Enable();
  }
  public void EnableLabActionMap() {
    actions.Lab.Enable();
    //TODO:nnewsom Do we need to disable before we enable action maps?
    // Can two action maps be active at the same time?
  }
}

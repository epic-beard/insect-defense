using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LabInputManager : MonoBehaviour {
  public static LabInputManager Instance;

  private LabInputs actions;
  private InputActionMap oldMap;
  private InputActionMap currMap;

  private void Awake() {
    Instance = this;
    actions = new();
  }

  private void Start() {
    actions.Lab.Enable();
    currMap = actions.Lab;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;

    actions.Lab.Lab_Settings.started += OnOpenSettings;

    actions.Selected.Selected_Back.started += OnCloseFocused;
    actions.Terrarium.Terrarium_Back.started += OnCloseTerrarium;
  }

  private void OnOpenSettings(InputAction.CallbackContext context) {
    UpdateActions(actions.SettingsScreen);
    SettingsScreen.Instance.OpenSettings();
  }
  private void OnCloseSettings(InputAction.CallbackContext context) {
    UpdateActions(oldMap);
    SettingsScreen.Instance.CloseSettings();
  }

  private void OnCloseFocused(InputAction.CallbackContext context) {
    ItemFocusScreen.Instance.CloseScreen();
  }

  private void OnCloseTerrarium(InputAction.CallbackContext context) {
    lab.LevelSelectScreen.Instance.CloseScreen();
  }

  public void EnableSelectedActionMap() {
    UpdateActions(actions.Selected);
  }
  public void EnableTerrariumActionMap() {
    UpdateActions(actions.Terrarium);
  }
  public void EnableLabActionMap() {
    UpdateActions(actions.Lab);
  }

  public void OnDisable() {
    actions.Disable();
  }

  private void UpdateActions(InputActionMap map) {
    oldMap = currMap;
    oldMap.Disable();

    currMap = map;
    currMap.Enable();
  }
}

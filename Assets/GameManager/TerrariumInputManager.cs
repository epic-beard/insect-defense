using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TerrariumInputManager : MonoBehaviour{
  private TerrariumInputs actions;
  public static TerrariumInputManager Instance;

  private void Awake() {
    actions = new();
    Instance = this;
  }

  private void Update() {
    HandleCameraMovement();
  }

  private void OnEnable() {
    actions.Player.Enable();
    actions.Player.Player_Pause.started += OnPauseGame;
    actions.Player.Player_Settings.started += OnOpenSettings;
    actions.Player.Player_Deselect.started += OnDeselect;
    actions.Player.Player_Zoom.started += OnZoomCamera;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  // TODO nnewsom add message box input map.

  void HandleCameraMovement() {
    Vector2 move = actions.Player.Player_Look.ReadValue<Vector2>();
    if (move.sqrMagnitude >= 0.1) {
      CameraManager.Instance.MoveCamera(move);
    }
  }

  void OnZoomCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ZoomCamera(context.ReadValue<Vector2>().y);
  }

  void OnPauseGame(InputAction.CallbackContext context) {
    PauseManager.Instance.HandlePause();
  }

  void OnDeselect(InputAction.CallbackContext context) {
    GameStateManager.Instance.ClearSelection();
    TerrariumContextUI.Instance.SetNoContextPanel();
  }

  protected void OnOpenSettings(InputAction.CallbackContext context) {
    OpenSettings();
  }
  public void OpenSettings() {
    PauseManager.Instance.HandleScreenPause();
    TerrariumUI.Instance.HideUI();
    SettingsScreen.Instance.OpenSettings();
    actions.Player.Disable();
    actions.SettingsScreen.Enable();
  }

  protected void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    // TODO nnewsom figure out closing settings to message box.
    PauseManager.Instance.HandleScreenPause();
    TerrariumUI.Instance.ShowUI();
    SettingsScreen.Instance.CloseSettings();
    actions.SettingsScreen.Disable();
    actions.Player.Enable();
  }

  public void ToggleSettings() {
    if (actions.SettingsScreen.enabled) {
      CloseSettings();
    } else {
      OpenSettings();
    }
  }
}

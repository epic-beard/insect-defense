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
    actions.Player.Player_Load.started += OnOpenLoadScreen;
    actions.Player.Player_Deselect.started += OnDeselect;
    actions.Player.Player_Zoom.started += OnZoomCamera;

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

  protected void OnOpenLoadScreen(InputAction.CallbackContext context) {
    PauseManager.Instance.HandleScreenPause();
    LoadScreen.Instance.OpenMenu();
    TerrariumUI.Instance.HideUI();
    actions.Player.Disable();
    actions.LoadScreen.Enable();
  }

  protected void OnCloseLoadScreen(InputAction.CallbackContext context) {
    PauseManager.Instance.HandleScreenPause();
    LoadScreen.Instance.CloseMenu();
    TerrariumUI.Instance.ShowUI();
    actions.LoadScreen.Disable();
    actions.Player.Enable();
  }

  protected void OnOpenSettings(InputAction.CallbackContext context) {
    OpenSettings();
  }
  public void OpenSettings() {
    PauseManager.Instance.HandleScreenPause();
    TerrariumUI.Instance.HideUI();
    SettingsScreen.Instance.ShowSettings();
    actions.Player.Disable();
    actions.SettingsScreen.Enable();
  }

  protected void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    PauseManager.Instance.HandleScreenPause();
    TerrariumUI.Instance.ShowUI();
    SettingsScreen.Instance.HideSettings();
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

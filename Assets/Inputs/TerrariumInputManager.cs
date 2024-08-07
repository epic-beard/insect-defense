using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrariumInputManager : MonoBehaviour {
  public static event Action OnAdvance = delegate { };

  public static TerrariumInputManager Instance;

  private TerrariumInputs actions;
  private InputActionMap? oldMap;
  private InputActionMap currMap;

  private void Awake() {
    actions = new();
    Instance = this;
  }

  private void Update() {
    HandleCameraMovement();
  }

  private void Start() {
    actions.Player.Enable();
    currMap = actions.Player;
    actions.Player.Player_Pause.started += PauseGame;
    actions.Player.Player_Settings.started += OpenSettings;
    actions.Player.Player_Deselect.started += Deselect;
    actions.Player.Player_Zoom.started += ZoomCamera;
    actions.Player.Player_Camera_Home.started += ResetCamera;
    actions.Player.Player_Delete_Tower.started += DeleteTower;
    actions.Player.Player_Turbo_Boost.started += ToggleTurboBoost;

    actions.MessageBox.Advance.started += Advance;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  void HandleCameraMovement() {
    Vector2 move = actions.Player.Player_Look.ReadValue<Vector2>();
    if (move.sqrMagnitude >= 0.1) {
      CameraManager.Instance.MoveCamera(move);
    }
    
    Vector2 rotation2D = actions.Player.Player_Camera_Rotate.ReadValue<Vector2>();
    if (!rotation2D.Equals(Vector2.zero)) {
      CameraManager.Instance.RotateCamera(rotation2D);
    }

    float elevationDelta = actions.Player.Player_Camera_Elevation.ReadValue<float>();
    if (elevationDelta != 0) {
      CameraManager.Instance.ChangeElevation(elevationDelta);
    }
  }

  void ZoomCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ZoomCamera(context.ReadValue<Vector2>().y);
  }

  void ResetCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ResetCamera();
  }

  void PauseGame(InputAction.CallbackContext context) {
    GameSpeedManager.Instance.HandlePause();
  }

  void Deselect(InputAction.CallbackContext context) {
    TowerManager.Instance.ClearSelection();
  }

  void DeleteTower(InputAction.CallbackContext context) {
    TowerManager.Instance.RefundSelectedTower();
  }

  void ToggleTurboBoost(InputAction.CallbackContext context) {
    GameSpeedManager.Instance.ToggleTurboBoost();
  }

  protected void OpenSettings(InputAction.CallbackContext context) {
    OpenSettings();
  }
  public void OpenSettings() {
    GameSpeedManager.Instance.Pause(PauseToken.SETTINGS);
    TerrariumScreen.Instance.HideUI();
    SettingsScreen.Instance.OpenSettings(inGame: true);
    UpdateActions(actions.SettingsScreen);
  }

  protected void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    GameSpeedManager.Instance.Unpause(PauseToken.SETTINGS);
    TerrariumScreen.Instance.ShowUI();
    SettingsScreen.Instance.CloseSettings();
    UpdateActions(oldMap);
  }

  public void ToggleSettings() {
    if (actions.SettingsScreen.enabled) {
      CloseSettings();
    } else {
      OpenSettings();
    }
  }

  private void Advance(InputAction.CallbackContext context) {
    OnAdvance.Invoke();
  }

  public void EnableMessageBoxActionMap() {
    actions.MessageBox.Enable();
  }

  public void DisableMessageBoxActionMap() {
    actions.MessageBox.Disable();
  }

  public void EnablePlayerActionMap() {
    UpdateActions(actions.Player);
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

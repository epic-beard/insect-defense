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

  private void OnEnable() {
    actions.Player.Enable();
    currMap = actions.Player;
    actions.Player.Player_Pause.started += PauseGame;
    actions.Player.Player_Settings.started += OpenSettings;
    actions.Player.Player_Deselect.started += Deselect;
    actions.Player.Player_Zoom.started += ZoomCamera;
    actions.Player.Player_Camera_Home.started += ResetCamera;

    actions.MessageBox.Advance.started += Advance;
    actions.MessageBox.Settings.started += OpenSettings;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  void HandleCameraMovement() {
    Vector2 move = actions.Player.Player_Look.ReadValue<Vector2>();
    if (move.sqrMagnitude >= 0.1) {
      CameraManager.Instance.MoveCamera(move);
    }

    float rotation = actions.Player.Player_Rotate.ReadValue<float>();
    CameraManager.Instance.RotateCamera(rotation);
  }

  void ZoomCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ZoomCamera(context.ReadValue<Vector2>().y);
  }

  void ResetCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ResetCamera();
  }

  void PauseGame(InputAction.CallbackContext context) {
    PauseManager.Instance.HandlePause();
  }

  void Deselect(InputAction.CallbackContext context) {
    GameStateManager.Instance.ClearSelection();
    TerrariumContextUI.Instance.SetNoContextPanel();
  }

  protected void OpenSettings(InputAction.CallbackContext context) {
    OpenSettings();
  }
  public void OpenSettings() {
    PauseManager.Instance.HandlePause(PauseToken.SETTINGS);
    TerrariumUI.Instance.HideUI();
    SettingsScreen.Instance.OpenSettings(inGame: true);
    UpdateActions(actions.SettingsScreen);
  }

  protected void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    PauseManager.Instance.HandlePause(PauseToken.SETTINGS);
    TerrariumUI.Instance.ShowUI();
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
    UpdateActions(actions.MessageBox);
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

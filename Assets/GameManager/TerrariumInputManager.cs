using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrariumInputManager : MonoBehaviour {
  public static event Action OnAdvance = delegate { };

  public static TerrariumInputManager Instance;

  private TerrariumInputs actions;

  private void Awake() {
    actions = new();
    Instance = this;
  }

  private void Update() {
    HandleCameraMovement();
  }

  private void OnEnable() {
    actions.Player.Enable();
    actions.Player.Player_Pause.started += PauseGame;
    actions.Player.Player_Settings.started += OpenSettings;
    actions.Player.Player_Deselect.started += Deselect;
    actions.Player.Player_Zoom.started += ZoomCamera;

    actions.MessageBox.Advance.started += Advance;
    actions.MessageBox.Settings.started += OpenSettings;

    actions.SettingsScreen.SettingsScreen_Close.started += OnCloseSettings;
  }

  void HandleCameraMovement() {
    Vector2 move = actions.Player.Player_Look.ReadValue<Vector2>();
    if (move.sqrMagnitude >= 0.1) {
      CameraManager.Instance.MoveCamera(move);
    }
  }

  void ZoomCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ZoomCamera(context.ReadValue<Vector2>().y);
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
    SettingsScreen.Instance.OpenSettings();
    if (MessageBox.Instance.IsOpen()) {
      actions.MessageBox.Disable();
    } else {
      actions.Player.Disable();
    }
    actions.SettingsScreen.Enable();
  }

  protected void OnCloseSettings(InputAction.CallbackContext context) {
    CloseSettings();
  }
  public void CloseSettings() {
    PauseManager.Instance.HandlePause(PauseToken.SETTINGS);
    TerrariumUI.Instance.ShowUI();
    SettingsScreen.Instance.CloseSettings();

    if (MessageBox.Instance.IsOpen()) {
      actions.SettingsScreen.Disable();
      actions.MessageBox.Enable();
    } else {
      actions.SettingsScreen.Disable();
      actions.Player.Enable();
    }
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
    actions.Player.Disable();
    actions.MessageBox.Enable();
  }

  public void DisableMessageBoxActionMap() {
    actions.MessageBox.Disable();
    actions.Player.Enable();
  }

  public void OnDisable() {
    actions.Disable();
  }
}

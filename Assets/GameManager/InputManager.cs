using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {
  public static InputManager Instance;
  private PlayerInput input;
  private InputAction playerLook;

  private void Awake() {
    Instance = this;
    input = GetComponent<PlayerInput>();
    playerLook = input.actions["Player_Look"];
  }

  private void Update() {
    HandleCameraMovement();
  }

  private void OnEnable() {
    SwitchToSceneActionMap();
    input.actions["Player_Pause"].started += PauseGame;
    input.actions["Player_Settings"].started += EnterSettingsScreen;
    input.actions["Player_Load"].started += EnterLoadScreen;
    input.actions["Player_Deselect"].started += Deselect;
    input.actions["Player_Zoom"].started += ZoomCamera;

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
  }

  void HandleCameraMovement() {
    Vector2 move = playerLook.ReadValue<Vector2>();
    if (move.sqrMagnitude >= 0.1) {
      CameraManager.Instance.MoveCamera(move);
    }
  }

  void ZoomCamera(InputAction.CallbackContext context) {
    CameraManager.Instance.ZoomCamera(context.ReadValue<float>());
  }

  void EnterSettingsScreen(InputAction.CallbackContext context) {
    SettingsScreen.Instance.OpenSettings();
  }

  void PauseGame(InputAction.CallbackContext context) {
    PauseManager.Instance.HandlePause();
  }

  void EnterLoadScreen(InputAction.CallbackContext context) {
    PauseManager.Instance.HandleScreenPause();
    input.SwitchCurrentActionMap("LoadScreen");
    LoadScreen.Instance.OpenMenu();
    TerrariumUI.Instance.HideUI();
  }

  void Deselect(InputAction.CallbackContext context) {
    GameStateManager.Instance.ClearSelection();
    TerrariumContextUI.Instance.SetNoContextPanel();
  }

  void ExitLoadScreen(InputAction.CallbackContext context) {
    PauseManager.Instance.HandleScreenPause();
    LoadScreen.Instance.CloseMenu();
    SwitchToSceneActionMap();
    TerrariumUI.Instance.ShowUI();
  }

  void ExitSettingsScreen(InputAction.CallbackContext context) {
    SettingsScreen.Instance.CloseSettings();
  }

  public void SwitchToSceneActionMap() {
    if (SceneManager.GetActiveScene().name == "Lab") {
      input.SwitchCurrentActionMap("Lab");
    } else {
      input.SwitchCurrentActionMap("Player");
    }
  }

  public void SwitchToActionMap(string action) {
    input.SwitchCurrentActionMap(action);
  }
}

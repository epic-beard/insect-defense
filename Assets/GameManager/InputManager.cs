using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {
  public static PlayerInput Input;

  private void Awake() {
    Input = GetComponent<PlayerInput>();
  }

  private void OnEnable() {
    SwitchToSceneActionMap();
    // [TODO: nnewsom] implement the camera movement.
    //Input.actions["Player_Look"].started += MoveCamera;
    Input.actions["Player_Pause"].started += PauseGame;
    Input.actions["Player_Settings"].started += EnterSettingsScreen;
    Input.actions["Player_Load"].started += EnterLoadScreen;
    Input.actions["Player_Deselect"].started += Deselect;
    //Input.actions["Player_Zoom"].started += ZoomCamera;

    // [TODO] input keyboard navigation of load screen.
    //Input.actions["LoadScreen_Navigate"].started += LoadScreenNavigate;
    //Input.actions["LoadScreen_Select"].started += LoadScreenSelect;
    Input.actions["LoadScreen_Close"].started += ExitLoadScreen;

    // [TODO] input keyboard navigation of settings screen.
    //Input.actions["SettingsScreen_Navigate"].started += SettingsScreenNavigate;
    //Input.actions["SettingsScreen_Select"].started += SettingsScreenSelect;
    //Input.actions["SettingsScreen_Back"].started += SettingsScreenBack;
    Input.actions["SettingsScreen_Close"].started += ExitSettingsScreen;

    Input.actions["Lab_Load"].started += EnterLoadScreen;
    Input.actions["Lab_Settings"].started += EnterSettingsScreen;
  }

  void EnterSettingsScreen(InputAction.CallbackContext context) {
    SettingsScreen.Instance.OpenSettings();
  }

  void PauseGame(InputAction.CallbackContext context) {
    PauseManager.Instance.HandlePause();
  }

  void EnterLoadScreen(InputAction.CallbackContext context) {
    PauseManager.Instance.HandleScreenPause();
    Input.SwitchCurrentActionMap("LoadScreen");
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

  public static void SwitchToSceneActionMap() {
    if (SceneManager.GetActiveScene().name == "Lab") {
      Input.SwitchCurrentActionMap("Lab");
    } else {
      Input.SwitchCurrentActionMap("Player");
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour {
  public static InputManager Instance;
  protected PlayerInput input;
  public void SwitchToSceneActionMap() {
    if (SceneManager.GetActiveScene().name == "StartScreen") {
      input.SwitchCurrentActionMap("StartScreen");
    } else if (SceneManager.GetActiveScene().name == "Lab") {
      input.SwitchCurrentActionMap("Lab");
    } else {
      input.SwitchCurrentActionMap("Player");
    }
  }
  public void SwitchToActionMap(string action) {
    input.SwitchCurrentActionMap(action);
  }
  public virtual void OpenSettings() {
    InputManager.Instance.SwitchToActionMap("SettingsScreen");
    SettingsScreen.Instance.ShowSettings();
  }

  public virtual void CloseSettings() {
    InputManager.Instance.SwitchToSceneActionMap();
    SettingsScreen.Instance.HideSettings();
  }
}

#nullable enable
using System;
using UnityEngine;

public class LabState : MonoBehaviour {
  public static LabState Instance;

  public bool IsFocused = false;
  private void Awake() {
    Instance = this;
  }

  public bool CanClickGameScreen() {
    return !SettingsScreen.Instance.IsOpen();
  }
}

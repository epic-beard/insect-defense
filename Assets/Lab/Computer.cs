using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
  [SerializeField] private Vector3 computerLocation;
  [SerializeField] private Quaternion computerRotation;

  private void OnMouseUp() {
    if (GameStateManager.Instance.IsMouseOverUI) return;
    if (!LabState.Instance.isFocused) {
      LabCamera.Instance.MoveTo(computerLocation, computerRotation);
      LabInputManager.Instance.EnableSelectedActionMap();
      LabState.Instance.isFocused = true;
    }
  }
}

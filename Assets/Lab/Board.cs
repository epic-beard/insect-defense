using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
  [SerializeField] private Vector3 boardLocation;
  [SerializeField] private Quaternion boardRotation;

  private void OnMouseUp() {
    if (GameStateManager.Instance.IsMouseOverUI) return;
    if (!LabState.Instance.isFocused) {
      LabCamera.Instance.MoveTo(boardLocation, boardRotation);
      LabInputManager.Instance.EnableSelectedActionMap();
      LabState.Instance.isFocused = true;
    }
  }
}

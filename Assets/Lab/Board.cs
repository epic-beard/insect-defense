using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
  [SerializeField] private Vector3 boardLocation;
  [SerializeField] private Quaternion boardRotation;

  private void OnMouseUp() {
    if (!LabState.Instance.CanClickGameScreen()) return;

    LabCamera.Instance.MoveToFocus(boardLocation, boardRotation);
  }
}

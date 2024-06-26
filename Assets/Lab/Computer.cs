using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
  [SerializeField] private Vector3 computerLocation;
  [SerializeField] private Quaternion computerRotation;

  private void OnMouseUp() {
    if (!LabState.Instance.CanClickGameScreen()) return;
  
    LabCamera.Instance.MoveToFocus(computerLocation, computerRotation);
  }
}

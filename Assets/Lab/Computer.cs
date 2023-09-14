using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
  [SerializeField] private Vector3 computerLocation;
  [SerializeField] private Quaternion computerRotation;

  private void OnMouseDown() {
    LabCamera.Instance.MoveTo(computerLocation, computerRotation);
    InputManager.Instance.SwitchToActionMap("Selected");
  }
}

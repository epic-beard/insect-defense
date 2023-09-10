using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
  [SerializeField] private Vector3 computerLocation;
  [SerializeField] private Quaternion computerRotation;
  private LabCamera labCamera;

  private void OnEnable() {
    labCamera = FindObjectOfType<LabCamera>();
  }

  private void OnMouseDown() {
    labCamera.MoveTo(computerLocation, computerRotation);
    InputManager.Instance.SwitchToActionMap("Selected");
  }
}

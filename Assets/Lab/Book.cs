using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {
  [SerializeField] private Vector3 bookLocation;
  [SerializeField] private Quaternion bookRotation;

  private void OnMouseUp() {
    if (!LabState.Instance.CanClickGameScreen()) return;

    LabCamera.Instance.MoveToFocus(bookLocation, bookRotation);
  }
}

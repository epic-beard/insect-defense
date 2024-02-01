using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {
  [SerializeField] private Vector3 bookLocation;
  [SerializeField] private Quaternion bookRotation;

  private void OnMouseDown() {
    if (!LabState.Instance.isFocused) {
      LabCamera.Instance.MoveTo(bookLocation, bookRotation);
      LabInputManager.Instance.EnableSelectedActionMap();
    }
  }
}
